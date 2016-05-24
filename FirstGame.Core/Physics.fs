namespace FirstGame.Core

open System
open System.Threading.Tasks
open Urho
open Urho.Gui
open Urho.Physics
open Urho.Urho2D

module Physics =

    type Vector2 with
        static member FromVector3 (vec3: Vector3) =
            new Vector2(vec3.X, vec3.Y)

    type Box() =
        inherit Component()
                
        member this.Play position scale bodyType =
            this.Node.Position <- position
            this.Node.Scale <- scale

            // Add a rigid body to the box
            let body = this.Node.CreateComponent<RigidBody2D>()
            body.BodyType <- bodyType
            
            // Add a collision shape to the box
            let collision = this.Node.CreateComponent<CollisionBox2D>()
            collision.Size <- Vector2.Multiply(Vector2.FromVector3 scale, new Vector2(0.32f, 0.32f))
            collision.Friction <- 0.5f

            // Set the sprite image
            let sprite = this.Node.CreateComponent<StaticSprite2D>()
            sprite.Sprite <- this.Application.ResourceCache.GetSprite2D("Images/Box.png")

    
    type Ball() =
        inherit Component()

        member this.Play position scale bodyType =
            this.Node.Position <- position
            this.Node.Scale <- scale

            // Add a rigid body to the box
            let body = this.Node.CreateComponent<RigidBody2D>()
            body.BodyType <- bodyType

            // Add a collision shape to the box
            let collision = this.Node.CreateComponent<CollisionCircle2D>()
            collision.Radius <- 0.16f
            collision.Density <- 1.0f
            collision.Friction <- 0.5f
            collision.Restitution <- 0.1f

            // Set the sprite image
            let sprite = this.Node.CreateComponent<StaticSprite2D>()
            sprite.Sprite <- this.Application.ResourceCache.GetSprite2D("Images/Ball.png")


    type App(opt: ApplicationOptions) =
        inherit Application(opt)

        let pixelSize = 0.01f
        
        override this.Start() =
            base.Start()
            
            let scene = new Scene()
            scene.CreateComponent<Octree>() |> ignore
            scene.CreateComponent<DebugRenderer>() |> ignore
            let physicsWorld2D = scene.CreateComponent<PhysicsWorld2D>()

            let cameraNode = scene.CreateChild("Camera")
            cameraNode.Position <- new Vector3(0.0f, 0.0f, -10.0f)

            let camera = cameraNode.CreateComponent<Camera>()
            camera.Orthographic <- true
            
            let graphics = base.Graphics
            camera.OrthoSize <- (float32 graphics.Height) * pixelSize

            let text = 
                new Text(
                    Value = sprintf "w:%i h:%i" graphics.Width graphics.Height,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom)
            text.SetColor (new Color(0.f, 1.f, 0.f))
            text.SetFont(base.ResourceCache.GetFont("Fonts/Anonymous Pro.ttf"), 20) |> ignore
            base.UI.Root.AddChild(text)
            
            base.UI.SubscribeToUIMouseClick(fun args -> text.Value <- "Button clicked") |> ignore

            let initialPosition = new Vector3(0.0f, 3.0f, 0.0f)
            let ball = new Ball()
            let ballNode = scene.CreateChild("Ball")
            ballNode.AddComponent(ball)
            ball.Play
                <| initialPosition
                <| new Vector3(1.0f, 1.0f, 0.0f)
                <| BodyType2D.Dynamic

            let box = new Box()
            let boxNode = scene.CreateChild("Ground")
            boxNode.AddComponent(box)
            box.Play
                <| new Vector3(0.0f, -4.0f, 0.0f)
                <| new Vector3(5.0f, 1.0f, 0.0f)
                <| BodyType2D.Kinematic
                      
            // check why collision is only detected one time      
            physicsWorld2D.SubscribeToPhysicsBeginContact2D(fun args ->
                let setVelocity (body: RigidBody2D) =
                    body.SetLinearVelocity(new Vector2(0.0f, 10.0f))
                
                match args.NodeA.Name, args.NodeB.Name with
                | "Ball", "Ground" -> setVelocity(args.BodyA)
                | "Ground", "Ball" -> setVelocity(args.BodyB)
                | _ -> ())
            |> ignore

            base.Renderer.SetViewport(uint32 0, new Viewport(base.Context, scene, cameraNode.GetComponent<Camera>(), null))