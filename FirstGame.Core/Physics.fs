namespace FirstGame.Core

open System
open System.Threading.Tasks
open Urho
open Urho.Gui
open Urho.Physics
open Urho.Urho2D

module Physics =

    type Scene with
        member scene.CreateSpriteNode name sprite2D position =
            let node = scene.CreateChild(name)
            let sprite = node.CreateComponent<StaticSprite2D>()
            node.Position <- position
            sprite.Sprite <- sprite2D
            node

    type Node with
        static member AddRigidBody2D bodyType (node: Node) =
            let body = node.CreateComponent<RigidBody2D>()
            body.BodyType <- bodyType
            node
        
        static member AddCollisionCircle2D (node: Node) =
            let c = node.CreateComponent<CollisionCircle2D>()
            c.Radius <- 0.16f
            c.Density <- 1.0f
            c.Friction  <- 0.5f
            c.Restitution <- 0.1f
            node
        
        static member AddCollisionBox2D (node: Node) =
            let c = node.CreateComponent<CollisionBox2D>()
            c.Size <- new Vector2(0.32f, 0.32f)
            node

    type Box() =
        inherit Component()

        member x.Play (text: Text) =
            x.Node.SubscribeToNodeCollisionStart(fun args -> text.Value <- "Collided!") |> ignore

    type App(opt: ApplicationOptions) =
        inherit Application(opt)

        let random = new Random()
        let pixelSize = 0.01f
        let negate x = x * -1.f
        
        override this.Start() =
            base.Start()
            
            let scene = new Scene()
            scene.CreateComponent<Octree>() |> ignore
            scene.CreateComponent<DebugRenderer>() |> ignore
            scene.CreateComponent<PhysicsWorld2D>() |> ignore

            let cameraNode = scene.CreateChild("Camera")
            cameraNode.Position <- new Vector3(0.0f, 0.0f, -10.0f)

            let camera = cameraNode.CreateComponent<Camera>()
            camera.Orthographic <- true
            
            let graphics = base.Graphics
            camera.OrthoSize <- (float32 graphics.Height) * pixelSize

            let cache = base.ResourceCache
            let ballSprite = cache.GetSprite2D("Images/Ball.png")
            let boxSprite = cache.GetSprite2D("Images/Box.png")
            
            let halfWidth = (float32 base.Graphics.Width) * 0.5f * pixelSize
            let halfHeight = (float32 base.Graphics.Height) * 0.5f * pixelSize

            let text = 
                new Text(
                    Value = sprintf "w:%i h:%i" graphics.Width graphics.Height,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom)
            text.SetColor (new Color(0.f, 1.f, 0.f))
            text.SetFont(cache.GetFont("Fonts/Anonymous Pro.ttf"), 20) |> ignore
            base.UI.Root.AddChild(text)
            
            base.UI.SubscribeToUIMouseClick(fun args -> text.Value <- "Button clicked") |> ignore

            let b = new Box()
            b.Play text
            let boxNode2 = scene.CreateChild("Box2")
            boxNode2.AddComponent(b)


            let ballNode = 
                (scene.CreateSpriteNode "Ball" 
                    <| cache.GetSprite2D("Images/Ball.png")
                    <| new Vector3(0.0f, 2.0f, 0.0f))
                |> Node.AddRigidBody2D BodyType2D.Dynamic
                |> Node.AddCollisionBox2D

            let boxNode = 
                (scene.CreateSpriteNode "Box" 
                    <| cache.GetSprite2D("Images/Box.png")
                    <| new Vector3(0.0f, -2.0f, 0.0f))
                |> Node.AddRigidBody2D BodyType2D.Kinematic
                |> Node.AddCollisionBox2D
            
            ballNode.SubscribeToNodeCollision(fun args -> text.Value <- "Collision!") |> ignore
            boxNode.SubscribeToNodeCollision(fun args -> text.Value <- "Collision!") |> ignore
            ballNode.SubscribeToNodeCollisionStart(fun args -> text.Value <- "Collision!") |> ignore
            boxNode.SubscribeToNodeCollisionStart(fun args -> text.Value <- "Collision!") |> ignore

            base.Renderer.SetViewport(uint32 0, new Viewport(base.Context, scene, cameraNode.GetComponent<Camera>(), null))