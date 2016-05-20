namespace FirstGame.Core

open System
open Urho
open Urho.Gui
open Urho.Physics
open Urho.Urho2D

module Physics =
    
    type App(opt: ApplicationOptions) =
        inherit Application(opt)

        let random = new Random()
        let nextRandom() = float32 <| random.NextDouble()
        let nextRandomRange range = nextRandom() * range
        let nextRandomBetween min max = nextRandom() * (max - min) + min
        let nextRandomBetweenInt min max = random.Next(min, max)

        override this.Start() =
            base.Start()
            
            let scene = new Scene()
            scene.CreateComponent<Octree>() |> ignore
            scene.CreateComponent<DebugRenderer>() |> ignore
            scene.CreateComponent<PhysicsWorld2D>() |> ignore

            let ballSprite = base.ResourceCache.GetSprite2D("Images/Ball.png")

            [0..99]
            |> List.iter(fun i ->
                let node = scene.CreateChild("RigidBody")
                node.Position <- new Vector3(nextRandomBetween -0.1f 0.1f, 5.f + (float32 i) * 0.4f, 0.f)
                let body = node.CreateComponent<RigidBody2D>()
                body.BodyType <- BodyType2D.Dynamic
                let sprite = node.CreateComponent<StaticSprite2D>()
                sprite.Sprite <- ballSprite
                let circle = node.CreateComponent<CollisionCircle2D>()
                circle.Radius <- 0.16f
                circle.Density <- 1.0f
                circle.Friction <- 0.5f
                circle.Restitution <- 0.1f)
            ()

            let cameraNode = scene.CreateChild("Camera")
            cameraNode.Position <- new Vector3(0.0f, 0.0f, -10.0f)
            let camera = cameraNode.CreateComponent<Camera>()
            camera.Orthographic <- true

            base.Renderer.SetViewport(uint32 0, new Viewport(scene, camera, null))