#region Copyright & License Information
/*
 * Copyright 2007-2018 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using OpenRA.Mods.Common.Orders;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.RA2.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.RA2.Orders
{
    public class EnterGarrisonOrderTargeter<GarrisonableInfo> : UnitOrderTargeter where GarrisonableInfo : ITraitInfo
    {
        readonly AlternateGarrisonMode mode;
        readonly Func<Actor, Actor, bool> canTarget;
        readonly Func<Actor, Actor, bool> useEnterCursor;

        public EnterGarrisonOrderTargeter(string order, int priority,
            Func<Actor, Actor, bool> canTarget, Func<Actor, Actor, bool> useEnterCursor, AlternateGarrisonMode mode)
            : base(order, priority, "enter", true, true)
        {
            this.canTarget = canTarget;
            this.useEnterCursor = useEnterCursor;
            this.mode = mode;
        }

        public override bool CanTargetActor(Actor self, Actor target, TargetModifiers modifiers, ref string cursor)
        {
            switch (mode)
            {
                case AlternateGarrisonMode.None:
                    return false;
                case AlternateGarrisonMode.Force:
                    if (!modifiers.HasModifier(TargetModifiers.ForceMove))
                        return false;
                    break;
                case AlternateGarrisonMode.Default:
                    if (modifiers.HasModifier(TargetModifiers.ForceMove))
                        return false;
                    break;
                case AlternateGarrisonMode.Always:
                    break;
            }

            if ((target.Owner.PlayerName == "Creeps" || target.Owner.PlayerName == "Neutral" || self.Owner.IsAlliedWith(target.Owner)) && target.Info.HasTraitInfo<GarrisonableInfo>())
            {
                cursor = useEnterCursor(self, target) ? "enter" : "enter-blocked";
                return true;
            }

            if (!self.Owner.IsAlliedWith(target.Owner) || !target.Info.HasTraitInfo<GarrisonableInfo>() || !canTarget(self, target) || target.Owner.PlayerName == "Creeps")
                return false;

            cursor = useEnterCursor(self, target) ? "enter" : "enter-blocked";
            return true;
        }

        public override bool CanTargetFrozenActor(Actor self, FrozenActor target, TargetModifiers modifiers, ref string cursor)
        {
			switch (mode)
			{
				case AlternateGarrisonMode.None:
					return false;
				case AlternateGarrisonMode.Force:
					if (!modifiers.HasModifier(TargetModifiers.ForceMove))
						return false;
					break;
				case AlternateGarrisonMode.Default:
					if (modifiers.HasModifier(TargetModifiers.ForceMove))
						return false;
					break;
				case AlternateGarrisonMode.Always:
					break;
			}
			if (target.Info.HasTraitInfo<GarrisonableInfo>())
                return true;

            return false;
        }
    }
}
