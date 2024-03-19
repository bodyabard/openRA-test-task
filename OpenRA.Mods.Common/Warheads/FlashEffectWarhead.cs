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

using OpenRA.GameRules;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Warheads
{
	[Desc("Used to trigger a FlashPostProcessEffect trait on the world actor.")]
	public class FlashEffectWarhead : Warhead
	{
		[Desc("Corresponds to `Type` from `FlashPostProcessEffect` on the world actor.")]
		public readonly string FlashType = null;

		[FieldLoader.Require]
		[Desc("Duration of the flashing, measured in ticks. Set to -1 to default to the `Length` of the `FlashPostProcessEffect`.")]
		public readonly int Duration = 0;

		public override void DoImpact(in Target target, WarheadArgs args)
		{
			foreach (var flash in args.SourceActor.World.WorldActor.TraitsImplementing<FlashPostProcessEffect>())
				if (flash.Info.Type == FlashType)
					flash.Enable(Duration);
		}
	}
}
