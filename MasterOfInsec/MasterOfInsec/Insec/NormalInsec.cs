﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace MasterOfInsec
{
    static class NormalInsec
    {
        public enum steps
        {
            Q1 = 0,
            Q2 = 1,
            WardJump = 2,
            Flash = 3,
            R = 4,

        }
        public static steps Steps;
        public static Obj_AI_Hero insecAlly;
        public static Obj_AI_Hero insecEnemy;
        static bool insecActive;
        public static void ResetInsecStats()
        {
            //     beforeall = false;
            insecActive = false;
            Steps = steps.Q1;
        }
        public static Vector3 Insecpos(Obj_AI_Hero ts)
        {
            return Game.CursorPos.Extend(ts.Position, Game.CursorPos.Distance(ts.Position) + 250);
        }
        public static Vector3 GetInsecPos(Obj_AI_Hero target)
        {
            if (Program.menu.Item("Mode").GetValue<StringList>().SelectedIndex == 0)
            {
                return WardJump.InsecposTower(target); // insec torre
                //  Game.PrintChat("");
            }
            else if (Program.menu.Item("Mode").GetValue<StringList>().SelectedIndex == 1)
            {
                return WardJump.InsecposToAlly(insecEnemy, insecAlly); //insec ally  
            }
            else if (Program.menu.Item("Mode").GetValue<StringList>().SelectedIndex == 2)
            {
                return WardJump.Insecpos(target); // insec normal
            }

            return WardJump.Insecpos(target);
        }
        public static void RCast(Obj_AI_Hero target)
        {
            //    Program.R.Cast(target);
            if (Program.R.Cast(target).IsCasted())
            {
                insecActive = false;
                Steps = steps.Q1;
            }

        }
        public static void Combo()
        {
            if (Program.menu.Item("OrbwalkInsec").GetValue<bool>())
                Program.Player.IssueOrder(GameObjectOrder.MoveTo, Program.Player.Position.Extend(Game.CursorPos, 150));
            if (!Program.R.IsReady()) return;
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);
            Do(target);
        }
        public static void Do(Obj_AI_Hero target)
        {
            if (insecActive == false)
            {
                if (Program.Q.IsReady() && Program.W.IsReady() && Program.R.IsReady() && Program.Player.Mana >= 130)
                    insecActive = true;
            }
            if (!insecActive) return;
            if (target.IsValidTarget(Program.Q.Range))
            {
                if (Steps == steps.Q1)
                {
                    if (Program.Q.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne")
                    {
                        if (Program.Q.CastIfHitchanceEquals(target, Combos.Combo.HitchanceCheck(Program.menu.Item("seth").GetValue<Slider>().Value)))
                            Steps = steps.Q2;
                    }

                }
                else if (Steps == steps.Q2) // hit second q
                {
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo")
                    {
                        if (Program.Q.Cast())
                        {
                            if (!WardJump.getBestWardItem().IsValidSlot() && Program.menu.Item("useflash").GetValue<bool>())
                            {
                                Steps = steps.Flash;
                            }
                            else
                            {
                                Steps = steps.WardJump;
                            }
                        }

                    }
                }
                else if (Steps == steps.WardJump) // put ward
                {
                    if (Program.Player.Distance(WardJump.getward(target)) <= 600 && Program.W.IsReady())
                    {
                        WardJump.JumpTo(GetInsecPos(target));


                    }
                }
                else if (Steps == steps.Flash) // hit w
                {
                    if (WardJump.Insecpos(target).Distance(Program.Player.Position) < 400)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), GetInsecPos(target));
                        Steps = steps.R;
                    }
                }
                else if (Steps == steps.R) // and hit the kick
                {
                    RCast(target);

                }
                else
                {
                    //    insecActive = false;
                    //         Steps = steps.Q1;
                }
            }
        }
    }
}