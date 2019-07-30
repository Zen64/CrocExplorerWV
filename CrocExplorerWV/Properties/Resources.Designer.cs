﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrocExplorerWV.Properties {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CrocExplorerWV.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die SamplerState Sampler : register(s0);
        ///Texture2D ShaderTexture : register(t0);
        ///struct VertexShaderOutput
        ///{
        ///    float4 Position : SV_Position;
        ///    float4 Color : COLOR;
        ///    float2 TextureUV : TEXCOORD0;
        ///};
        ///float4 main(VertexShaderOutput input) : SV_Target
        ///{
        ///    return ShaderTexture.Sample(Sampler, input.TextureUV);
        ///} ähnelt.
        /// </summary>
        internal static string pixelShaderTextured {
            get {
                return ResourceManager.GetString("pixelShaderTextured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die struct VertexShaderInput
        ///{
        ///    float4 Position : SV_POSITION;
        ///    float4 Color : COLOR;
        ///};
        ///
        ///float4 main(VertexShaderInput input) : SV_TARGET 
        ///{ 
        ///    return input.Color; 
        ///} ähnelt.
        /// </summary>
        internal static string pixelShaderWired {
            get {
                return ResourceManager.GetString("pixelShaderWired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die cbuffer PerObject: register(b0)
        ///{
        ///    float4x4 WorldViewProj;
        ///};
        ///
        ///struct VertexShaderInput
        ///{
        ///    float4 Position : SV_Position;
        ///    float4 Color : COLOR;
        ///    float2 TextureUV : TEXCOORD0;
        ///};
        ///
        ///struct VertexShaderOutput
        ///{
        ///    float4 Position : SV_Position;
        ///    float4 Color : COLOR;
        ///    float2 TextureUV : TEXCOORD0;
        ///};
        ///
        ///VertexShaderOutput main(VertexShaderInput input)
        ///{
        ///    VertexShaderOutput output = (VertexShaderOutput)0;
        ///    output.Position = mul(input.Position, WorldViewProj);
        ///    output.TextureUV = input. [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        /// </summary>
        internal static string vertexShaderTextured {
            get {
                return ResourceManager.GetString("vertexShaderTextured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die cbuffer PerObject: register(b0)
        ///{
        ///    float4x4 WorldViewProj;
        ///};
        ///
        ///struct VertexShaderInput
        ///{
        ///    float4 Position : SV_Position;
        ///    float4 Color : COLOR;
        ///};
        ///
        ///struct VertexShaderOutput
        ///{
        ///    float4 Position : SV_Position;
        ///    float4 Color : COLOR;
        ///};
        ///
        ///VertexShaderOutput main(VertexShaderInput input)
        ///{
        ///    VertexShaderOutput output = (VertexShaderOutput)0;
        ///    output.Position = mul(input.Position, WorldViewProj);
        ///    output.Color = input.Color;
        ///    return output;
        ///} ähnelt.
        /// </summary>
        internal static string vertexShaderWired {
            get {
                return ResourceManager.GetString("vertexShaderWired", resourceCulture);
            }
        }
    }
}
