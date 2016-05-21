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
        let pixelSize = 0.01f

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

            let halfWidth = (float32 graphics.Width) * 0.5f * pixelSize
            let halfHeight = (float32 graphics.Height) * 0.5f * pixelSize

            [0..99]
            |> List.iter(fun i ->
                let node = scene.CreateChild("StaticSprite2D")
                node.Position <- new Vector3(nextRandomBetween (-1.0f * halfWidth) halfWidth, nextRandomBetween (-1.0f * halfHeight) halfHeight, 0.f)
                let sprite = node.CreateComponent<StaticSprite2D>()
                sprite.Sprite <- ballSprite)

            base.Renderer.SetViewport(uint32 0, new Viewport(base.Context, scene, cameraNode.GetComponent<Camera>(), null))