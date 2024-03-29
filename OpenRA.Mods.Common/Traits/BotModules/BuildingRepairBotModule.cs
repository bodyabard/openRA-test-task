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

using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
	[TraitLocation(SystemActors.Player)]
	[Desc("Manages AI repairing base buildings.")]
	public class BuildingRepairBotModuleInfo : ConditionalTraitInfo
	{
		public override object Create(ActorInitializer init) { return new BuildingRepairBotModule(this); }
	}

	public class BuildingRepairBotModule : ConditionalTrait<BuildingRepairBotModuleInfo>, IBotRespondToAttack
	{
		public BuildingRepairBotModule(BuildingRepairBotModuleInfo info)
			: base(info) { }

		void IBotRespondToAttack.RespondToAttack(IBot bot, Actor self, AttackInfo e)
		{
			// HACK: We don't want D2k bots to repair all their buildings on placement
			// where half their HP is removed via neutral terrain damage.
			// TODO: Implement concrete placement for D2k bots and remove this hack on players relationship check.
			if (self.IsDead || self.Owner.RelationshipWith(e.Attacker.Owner) == PlayerRelationship.Neutral)
				return;

			var rb = self.TraitOrDefault<RepairableBuilding>();
			if (rb != null && e.DamageState > DamageState.Light && e.PreviousDamageState <= DamageState.Light && !rb.RepairActive)
			{
				AIUtils.BotDebug("{0} noticed damage {1} {2}->{3}, repairing.",
					self.Owner, self, e.PreviousDamageState, e.DamageState);
				bot.QueueOrder(new Order("RepairBuilding", self.Owner.PlayerActor, Target.FromActor(self), false));
			}
		}
	}
}
