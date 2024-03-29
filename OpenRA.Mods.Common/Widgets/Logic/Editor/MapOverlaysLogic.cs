#region Copyright & License Information
/*
 * Copyright (c) The OpenRA Developers and Contributors
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Lint;
using OpenRA.Mods.Common.Traits;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.Logic
{
	[ChromeLogicArgsHotkeys("ToggleGridOverlayKey", "ToggleBuildableOverlayKey")]
	public class MapOverlaysLogic : ChromeLogic
	{
		[Flags]
		enum MapOverlays
		{
			None = 0,
			Grid = 1,
			Buildable = 2,
		}

		readonly TerrainGeometryOverlay terrainGeometryTrait;
		readonly BuildableTerrainOverlay buildableTerrainTrait;
		readonly Widget widget;

		[ObjectCreator.UseCtor]
		public MapOverlaysLogic(Widget widget, World world, ModData modData, Dictionary<string, MiniYaml> logicArgs)
		{
			this.widget = widget;

			terrainGeometryTrait = world.WorldActor.Trait<TerrainGeometryOverlay>();
			buildableTerrainTrait = world.WorldActor.Trait<BuildableTerrainOverlay>();

			var toggleGridKey = new HotkeyReference();
			if (logicArgs.TryGetValue("ToggleGridOverlayKey", out var yaml))
				toggleGridKey = modData.Hotkeys[yaml.Value];

			var toggleBuildableKey = new HotkeyReference();
			if (logicArgs.TryGetValue("ToggleBuildableOverlayKey", out yaml))
				toggleBuildableKey = modData.Hotkeys[yaml.Value];

			var keyhandler = widget.Get<LogicKeyListenerWidget>("OVERLAY_KEYHANDLER");
			keyhandler.AddHandler(e =>
			{
				if (e.Event != KeyInputEvent.Down)
					return false;

				if (toggleGridKey.IsActivatedBy(e))
				{
					terrainGeometryTrait.Enabled ^= true;
					return true;
				}

				if (toggleBuildableKey.IsActivatedBy(e))
				{
					buildableTerrainTrait.Enabled ^= true;
					return true;
				}

				return false;
			});

			var overlayPanel = CreateOverlaysPanel();

			var overlayDropdown = widget.GetOrNull<DropDownButtonWidget>("OVERLAY_BUTTON");
			if (overlayDropdown != null)
			{
				overlayDropdown.OnMouseDown = _ =>
				{
					overlayDropdown.RemovePanel();
					overlayDropdown.AttachPanel(overlayPanel);
				};
			}
		}

		Widget CreateOverlaysPanel()
		{
			var categoriesPanel = widget.Get<Widget>("TOOLS_WIDGETS");
			var showGridCheckbox = categoriesPanel.Get<CheckboxWidget>("SHOW_TILE_GRID");
			var showBuildableAreaCheckbox = categoriesPanel.Get<CheckboxWidget>("SHOW_BUILDABLE_AREA");

			MapOverlays[] allCategories = { MapOverlays.Grid, MapOverlays.Buildable };
			foreach (var cat in allCategories)
			{
				if (cat.HasFlag(MapOverlays.Grid))
				{
					showGridCheckbox.IsChecked = () => terrainGeometryTrait.Enabled;
					showGridCheckbox.OnClick = () => terrainGeometryTrait.Enabled ^= true;
				}
				else if (cat.HasFlag(MapOverlays.Buildable))
				{
					showBuildableAreaCheckbox.IsChecked = () => buildableTerrainTrait.Enabled;
					showBuildableAreaCheckbox.OnClick = () => buildableTerrainTrait.Enabled ^= true;
				}
			}

			return categoriesPanel;
		}
	}
}
