using ExampleMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Projectiles.Minions
{
	// ExampleHoveringMinion uses inheritace as an example of how it can be useful in modding.
	// HoverShooter and Minion classes help abstract common functionality away, which is useful for mods that have many similar behaviors.
	// Inheritance is an advanced topic and could be confusing to new programmers, see ExampleSimpleMinion.cs for a simpler minion example.
	public class ExampleHoveringMinion : HoverShooter
	{
		public override void SetStaticDefaults() {
			Main.projFrames[projectile.type] = 3; //how many frames there are in ExampleHoveringMinion's animation
			Main.projPet[projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true; //can cancel the minion
			ProjectileID.Sets.Homing[projectile.type] = true; //minion homes into enemies
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true; //This is necessary for right-click targeting
		}

		public override void SetDefaults() {
			projectile.netImportant = true;
			projectile.width = 24;
			projectile.height = 32;
			projectile.friendly = true; //ExampleHoveringMinion can't harm player
			projectile.minion = true; //sets the ExampleHoveringMinion to a minion
			projectile.minionSlots = 1; //ExampleHoveringMinion takes one minion slot
			projectile.penetrate = -1;
			projectile.timeLeft = 18000;
			projectile.tileCollide = false; //ExampleHoveringMinion can move through blocks
			projectile.ignoreWater = true; //ExampleHoveringMinion's movement ignores water
			inertia = 20f;
			shoot = ModContent.ProjectileType<PurityBolt>(); //ExampleHoveringMinion fires PurityBolt
			shootSpeed = 12f;
		}

		public override void CheckActive() {
			Player player = Main.player[projectile.owner];
			ExamplePlayer modPlayer = player.GetModPlayer<ExamplePlayer>();
			//makes sure ExampleHoveringMinion despawns when the player dies
			if (player.dead) {
				modPlayer.exampleHoveringMinion = false; 
			}
			if (modPlayer.exampleHoveringMinion) { // Make sure you are resetting this bool in ModPlayer.ResetEffects. See ExamplePlayer.ResetEffects
				projectile.timeLeft = 2;
			}
		}

		public override void CreateDust() {
			//creates the dust effect
			if (projectile.ai[0] == 0f) { //if ExampleHoveringMinion isn't moving
				if (Main.rand.NextBool(5)) {
					int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height / 2, ModContent.DustType<PuriumFlame>()); //ExampleHoveringMinion uses the PuriumFlame dust
					Main.dust[dust].velocity.Y -= 1.2f;
				}
			}
			else {
				if (Main.rand.NextBool(3)) {
					Vector2 dustVel = projectile.velocity;
					if (dustVel != Vector2.Zero) {
						dustVel.Normalize();
					}
					int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<PuriumFlame>()); //ExampleHoveringMinion uses the PuriumFlame dust
					Main.dust[dust].velocity -= 1.2f * dustVel; //this makes the dust effect take into account the movement of ExampleHoveringMinion
				}
			}
			//causes the ExampleHoveringMinion to emit light
			Lighting.AddLight((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f), 0.6f, 0.9f, 0.3f);
		}

		public override void SelectFrame() {
			//creates the projectile animation
			projectile.frameCounter++;
			if (projectile.frameCounter >= 8) {
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % 3;
			}
		}
	}
}
