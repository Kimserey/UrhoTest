namespace FirstGame.Core

open Urho
open Urho.Gui

module Game =
    
    type App(opt: ApplicationOptions) =
        inherit Application(opt)

        override this.Start() =
            base.Start()
            
            let cache = this.ResourceCache
            let helloText = 
                new Text(
                    Value = "Hello World from Urho3D, Mono, and F#",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center)
                    
            helloText.SetColor (new Color(0.f, 1.f, 0.f))
            
            let f = cache.GetFont("Fonts/Anonymous Pro.ttf")
            helloText.SetFont(f, 30) |> ignore
            
            this.UI.Root.AddChild(helloText)