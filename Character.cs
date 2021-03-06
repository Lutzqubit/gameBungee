﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace gameBungee
{
    public class Character
    {
        private World worldPhysic;
        //private BasicEffect _effectPlayer;

        //ENlever le rectanglePhysique, il sert a rien, adapter le code (body dans le constructeur de Circle)
        private ObjRectange RectangleTexture;
        private ObjRectange RectangleTextureGun;
        public  ObjCercle   ObjectPhysicCircle;
        public  ObjCercle   ObjectPhysicCircleTete;
        public  Bullet      bulletJuncture;

        //Boolean varaibles to know what the player is doing
        public Boolean isMoving         = false;
        public Boolean isMovingRight    = false;
        public Boolean isJumping        = false;
        public Boolean isShooting       = false;

        public Boolean oldMoving        = false;
        public Boolean oldMovingRight   = false;
        public Boolean oldJumping       = false;
        public Boolean oldShooting      = false;
        
        private Texture2D texJump;
        private Texture2D texRun;
        private Texture2D texIdle;
        private Texture2D texWeapon;

        private Boolean Flip            = false;
        public double angleTir          = 0.0d;
        public float  distanceTir       = 0.0f;

        public Character(Texture2D tex, World worldPhysic, ContentManager Content, BasicEffect effectPlayer, Vector2 position)
        {
            this.worldPhysic = worldPhysic;

            texJump = Content.Load<Texture2D>("textures/Run");
            texRun  = Content.Load<Texture2D>("textures/Run");
            texIdle = Content.Load<Texture2D>("textures/Idle");
            texWeapon = Content.Load<Texture2D>("textures/Weapon");

            //Cercle qui représente la tete
            ObjectPhysicCircleTete = new ObjCercle(new Vector2(0, 0), 2.0f, 0.0f, worldPhysic, BodyType.Dynamic);
            ObjectPhysicCircleTete.addDisplayer();
            ObjectPhysicCircleTete.FixtureObject.Friction = 1.0f;
            ObjectPhysicCircleTete.FixtureObject.Restitution = 0.0f;
            ObjectPhysicCircleTete.FixtureObject.CollisionFilter.CollisionCategories = Category.Cat20;
            //ObjectPhysicCircleTete.FixtureObject.CollisionFilter.RemoveCollidesWithCategory(Category.Cat30);

            //Cercle qui permet les déplacement
            ObjectPhysicCircle = new ObjCercle(new Vector2(0, -3.5f), 1.5f, 0.0f, worldPhysic, ObjectPhysicCircleTete.FixtureObject.Body);//, RectanglePhysic.FixtureObject.Body);
            ObjectPhysicCircle.addDisplayer();
            ObjectPhysicCircle.FixtureObject.Friction = 0.8f;
            ObjectPhysicCircle.FixtureObject.Restitution = 0.0f;
            ObjectPhysicCircle.FixtureObject.CollisionFilter.CollisionCategories = Category.Cat20;
            //ObjectPhysicCircle.FixtureObject.CollisionFilter.RemoveCollidesWithCategory(Category.Cat30);

            

            //Rectangle qui permet d'afficher le charactere
            RectangleTexture = new ObjRectange(new Vector2(0, 0), 10, 10, 0.0f, BodyType.Dynamic, ObjectPhysicCircle.FixtureObject.Body/*ObjectPhysicCircle.FixtureObject.Body*/);
            //RectangleTexture.addDisplayer();
            RectangleTexture.FixtureObject.Body.FixedRotation = true;
            RectangleTexture.addTexture(texIdle, 0.0f);
            //RectangleTexture.FixtureObject.CollisionFilter.RemoveCollisionCategory(Category.All);
            RectangleTexture.FixtureObject.CollisionFilter.RemoveCollidesWithCategory(Category.All);

            //Rectangle qui permet d'afficher le bras et l'arme

            RectangleTextureGun = new ObjRectange(new Vector2(0, 0), 10, 10, 0.0f, BodyType.Dynamic);
            //RectangleTextureGun.addDisplayer();
            RectangleTextureGun.FixtureObject.Body.FixedRotation = true;
            RectangleTextureGun.addTexture(texWeapon, 0.5f);
            //RectangleTextureGun.FixtureObject.CollisionFilter.RemoveCollisionCategory(Category.All);
            RectangleTextureGun.FixtureObject.CollisionFilter.RemoveCollidesWithCategory(Category.All);
            JointFactory.CreateRevoluteJoint(worldPhysic, RectangleTexture.FixtureObject.Body, RectangleTextureGun.FixtureObject.Body, Vector2.Zero);

            //ObjectPhysicCircleTete.FixtureObject.Body.Position = position;
        }

        public Object PlayerPhysic
        {
            get { return ObjectPhysicCircleTete; }
        }

        public virtual void joinWithObject(Fixture Obj2, Vector2 AnchorPoint2, float minLength, float maxLength, World worldPhysic)
        {
         //   juncture = JointFactory.CreateSliderJoint(worldPhysic, ObjectPhysicCircleTete.FixtureObject.Body, Obj2.Body, AnchorPoint, AnchorPoint2, minLength, maxLength);
        }

        public void update()
        {
			if (isShooting)
            {
                if (bulletJuncture != null)
                    bulletJuncture.remove();

                float radius = 0.5f;
                bulletJuncture = new Bullet(ObjectPhysicCircleTete.FixtureObject.Body.Position, radius, ObjectPhysicCircleTete.FixtureObject, worldPhysic);
                bulletJuncture.shootBullet(Flip, angleTir);
                isShooting = false;
            }
            if (bulletJuncture != null)
            {
                //distanceTir = (ObjectPhysicCircleTete.Position - bulletJuncture.positionBullet).Length();
                bulletJuncture.update(ObjectPhysicCircleTete.FixtureObject.Body.Position,distanceTir);
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (bulletJuncture != null)
                bulletJuncture.Draw(EnvironmentVariable.graphics.GraphicsDevice);
            if (isMoving)
            {
                Flip = isMovingRight;
                if (isJumping)
                {
                    if (RectangleTexture.A_AnimationChar.Animation.Texture != texRun)
                        RectangleTexture.addTexture(texJump, 0.05f);
                    else
                        CameraController.PlayerTexture = texJump;
                    RectangleTexture.Draw(gameTime, Flip);
                }
                else
                {
                    if (isMovingRight)
                    {
                        if (RectangleTexture.A_AnimationChar.Animation.Texture != texRun)
                            RectangleTexture.addTexture(texRun, 0.03f);
                        else
                            CameraController.PlayerTexture = texRun;
                        RectangleTexture.Draw(gameTime, Flip);
                    }
                    else //is moving left
                    {
                        if (RectangleTexture.A_AnimationChar.Animation.Texture != texRun)
                            RectangleTexture.addTexture(texRun, 0.05f);
                        else
                            CameraController.PlayerTexture = texRun;
                        RectangleTexture.Draw(gameTime, Flip);
                    }
                }
            }
            else
            {
                Flip = isMovingRight;
                if (RectangleTexture.A_AnimationChar.Animation.Texture != texIdle)
                    RectangleTexture.addTexture(texIdle, 0.0f);
                else
                    CameraController.PlayerTexture = texIdle;
                RectangleTexture.Draw(gameTime, Flip);

            }

            RectangleTextureGun.FixtureObject.Body.Rotation = (float)angleTir;
            if (RectangleTextureGun.A_AnimationChar.Animation.Texture != texWeapon)
                RectangleTextureGun.addTexture(texWeapon, 0.05f);
            else
                CameraController.PlayerTexture = texWeapon;
            RectangleTextureGun.Draw(gameTime, Flip);
            
        }
    }
}
