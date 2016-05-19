namespace FirstGame.Core

open System.Threading.Tasks
open Urho
open Urho.Gui
open Urho.Physics
open Urho.Actions

module Game =
    
    type App(opt: ApplicationOptions) =
        inherit Application(opt)

        override this.Start() =
            let cache = this.ResourceCache

            let helloText = 
                new Text(
                    Value = "Hello World from Urho3D, Mono, and F#",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center)

            helloText.SetColor (new Color(0.f, 1.f, 0.f))
            this.UI.Root.AddChild(helloText)