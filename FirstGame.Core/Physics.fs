namespace FirstGame.Core

open System
open System.Threading.Tasks
open Urho
open Urho.Gui
open Urho.Physics
open Urho.Urho2D

module Physics =

    type Box() =
        inherit Component()
                
        member this.Play (text: Text) position scale isKinematic =
            this.Node.Position <- position
            this.Node.Scale <- scale
            // Add a rigid body to the box
            let body = this.Node.CreateComponent<RigidBody>()
            body.Mass <- 1.0f
            body.Kinematic <- isKinematic

            // Add a collision shape to the box
            let collision = this.Node.CreateComponent<CollisionShape>()
            collision.SetBox(Vector3.Multiply(new Vector3(0.32f, 0.33f, 0.32f), scale), Vector3.Zero, Quaternion.Identity)

            // Set the sprite image
            let sprite = this.Node.CreateComponent<StaticSprite2D>()
            sprite.Sprite <- this.Application.ResourceCache.GetSprite2D("Images/Box.png")


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
            let box= new Box()
            let boxNode = scene.CreateChild("Box")
            boxNode.AddComponent(box)
            box.Play text
                <| initialPosition
                <| new Vector3(1.0f, 1.0f, 1.0f)
                <| false

            boxNode.SubscribeToNodeCollision(fun args ->
                match args.OtherNode.Name with
                | "Ground" -> 
                    args.Body.Node.Position <- initialPosition
                    args.Body.SetLinearVelocity(Vector3.Zero)
                    args.Body.SetAngularVelocity(Vector3.Zero)
                | _ -> ()) 
            |> ignore

            let ground = new Box()
            let groundNode = scene.CreateChild("Ground")
            groundNode.AddComponent(ground)
            ground.Play text
                <| new Vector3(0.0f, -3.0f, 0.0f)
                <| new Vector3(3.0f, 1.0f, 1.0f)
                <| true
                            
            base.Renderer.SetViewport(uint32 0, new Viewport(base.Context, scene, cameraNode.GetComponent<Camera>(), null))