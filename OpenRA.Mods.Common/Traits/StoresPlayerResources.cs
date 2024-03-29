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

using OpenRA.Primitives;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
	[Desc("Adds capacity to a player's harvested resource limit.")]
	public class StoresPlayerResourcesInfo : TraitInfo
	{
		[FieldLoader.Require]
		public readonly int Capacity = 0;

		public override object Create(ActorInitializer init) { return new StoresPlayerResources(init.Self, this); }
	}

	public class StoresPlayerResources : INotifyOwnerChanged, INotifyCapture, INotifyKilled, INotifyAddedToWorld, INotifyRemovedFromWorld
	{
		readonly StoresPlayerResourcesInfo info;
		PlayerResources player;

		public int Stored => player.ResourceCapacity == 0 ? 0 : (int)((long)info.Capacity * player.Resources / player.ResourceCapacity);

		public StoresPlayerResources(Actor self, StoresPlayerResourcesInfo info)
		{
			this.info = info;
			player = self.Owner.PlayerActor.Trait<PlayerResources>();
		}

		void INotifyOwnerChanged.OnOwnerChanged(Actor self, Player oldOwner, Player newOwner)
		{
			player = newOwner.PlayerActor.Trait<PlayerResources>();
		}

		void INotifyCapture.OnCapture(Actor self, Actor captor, Player oldOwner, Player newOwner, BitSet<CaptureType> captureTypes)
		{
			var resources = Stored;
			oldOwner.PlayerActor.Trait<PlayerResources>().TakeResources(resources);
			newOwner.PlayerActor.Trait<PlayerResources>().GiveResources(resources);
		}

		void INotifyKilled.Killed(Actor self, AttackInfo e)
		{
			// Lose the stored resources.
			player.TakeResources(Stored);
		}

		void INotifyAddedToWorld.AddedToWorld(Actor self)
		{
			player.AddStorageCapacity(info.Capacity);
		}

		void INotifyRemovedFromWorld.RemovedFromWorld(Actor self)
		{
			player.RemoveStorageCapacity(info.Capacity);
		}
	}
}
