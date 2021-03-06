using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Spells
{
    public class YasuoQW : IGameScript
    {
        private Vector2 trueCoords;
        public void OnActivate(IObjAiBase owner)
        {
            // here's nothing
        }

        public void OnDeactivate(IObjAiBase owner)
        {
            // here's empty
        }

        public void OnStartCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            var current = new Vector2(owner.Position.X, owner.Position.Y);
            var to = Vector2.Normalize(new Vector2(spell.X, spell.Y) - current);
            var range = to * spell.SpellData.CastRangeDisplayOverride[0];
            trueCoords = current + range;

            FaceDirection(owner, trueCoords, true, 0f);
        }

        public void OnFinishCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            if (HasBuff(owner, "YasuoE"))
            {
                spell.SpellAnimation("SPELL3b", owner);
                AddParticleTarget(owner, "Yasuo_Base_EQ_cas.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_EQ_SwordGlow.troy", owner, 1, "C_BUFFBONE_GLB_Weapon_1");
                foreach (var affectEnemys in GetUnitsInRange(owner.Position, 270f, true))
                {
                    if (affectEnemys is IAttackableUnit && affectEnemys.Team != owner.Team)
                    {
                        affectEnemys.TakeDamage(owner, spell.Level * 20f + owner.Stats.AttackDamage.Total, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                        AddParticleTarget(owner, "Yasuo_Base_Q_hit_tar.troy", affectEnemys);
                    }
                }
                AddBuff("YasuoQ01", 6f, 1, spell, owner, owner);
            }
            else
            {
                spell.SpellAnimation("SPELL1A", owner);
                spell.AddLaser("YasuoQ", trueCoords);
                AddParticleTarget(owner, "Yasuo_Q_Hand.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_Q1_cast_sound.troy", owner);
            }
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
            AddParticleTarget(owner, "Yasuo_Base_Q_hit_tar.troy", target);
            target.TakeDamage(owner, spell.Level * 20f + owner.Stats.AttackDamage.Total,DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            if (!HasBuff(owner, "YasuoQ01"))
            {
                AddBuff("YasuoQ01", 6f, 1, spell, owner, owner);
            }
        }

        public void OnUpdate(double diff)
        {
            //here's nothing
        }
    }
}
