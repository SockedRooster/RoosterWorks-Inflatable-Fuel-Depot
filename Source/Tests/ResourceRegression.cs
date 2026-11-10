using System;
using System.Collections.Generic;
using System.Reflection;
using InflataDepot;
using B9PartSwitch;
[AttributeUsage(AttributeTargets.Field)] public class KSPField:Attribute {public bool isPersistant,guiActive,guiActiveEditor;public string guiName;}
[AttributeUsage(AttributeTargets.Method)] public class KSPEvent:Attribute {public bool guiActive,guiActiveEditor,active;public string guiName;}
[AttributeUsage(AttributeTargets.Method)] public class KSPAction:Attribute {public KSPAction(string s){}}
public class KSPActionParam {}
public class BaseEvent {public bool active=true,guiActive=true,guiActiveEditor=true,guiActiveUnfocused=true;}
public class BaseAction {public bool active=true;}
public class Lookup<T> where T:new() {Dictionary<string,T> d=new Dictionary<string,T>();public T this[string s] {get {if(!d.ContainsKey(s))d[s]=new T();return d[s];}}}
public class PartResource {public string resourceName;public double amount,maxAmount;public bool flowState=true;}
public class Resources:List<PartResource> {public PartResource Get(string s){return Find(x=>x.resourceName==s);}}
public class Part {public Resources Resources=new Resources();public List<PartModule> Modules=new List<PartModule>();public bool fuelCrossFeed;}
public class PartModule {public enum StartState {None} public Part part;public Lookup<BaseEvent> Events=new Lookup<BaseEvent>();public Lookup<BaseAction> Actions=new Lookup<BaseAction>();public virtual void OnStart(StartState s){}public virtual void OnUpdate(){}public virtual void OnFixedUpdate(){}}
public class ModuleAnimateGeneric:PartModule {public string animationName="inflate";public float animTime;public bool moving;public int toggles;public bool IsMoving(){return moving;}public void Toggle(){toggles++;moving=true;}public void SetScalar(float s){animTime=s;}public UnityEngine.AnimationState GetState(){return new UnityEngine.AnimationState();}}
public class ModuleDockingNode:PartModule {public bool crossfeed,stagingEnabled,stagingToggleEnabledEditor,stagingToggleEnabledFlight;public void EnableXFeed(){crossfeed=true;part.fuelCrossFeed=true;}public void SetStaging(bool v){stagingEnabled=v;}}
public static class HighLogic {public static bool LoadedSceneIsFlight=true,LoadedSceneIsEditor;}
namespace UnityEngine {public class AnimationState {public float speed;}public static class Debug {public static void Log(string s){}public static void LogWarning(string s){}}}
namespace B9PartSwitch {
public class TankResource {public string ResourceName;public float unitsPerVolume;}
public class PartSubtype {}
public class TankType {public List<TankResource> resources=new List<TankResource>();}
public class ModuleB9PartSwitch:PartModule {public string moduleID="InflataFuel";public PartSubtype CurrentSubtype=new PartSubtype();public TankType CurrentTankType=new TankType();public float volume;public bool IsManagedResource(string s){return CurrentTankType.resources.Exists(x=>x.ResourceName==s);}public float GetTotalVolume(PartSubtype s){return volume;}}
}
public class Regression {
static int count;
static void Check(bool ok,string msg){count++;if(!ok)throw new Exception(msg);}
static void Call(object o,string name){o.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(o,null);}
static void Main(){
 foreach(float volume in new[]{2250f,18000f,60750f})
 foreach(string spec in new[]{"LiquidFuel:0.45,Oxidizer:0.55","LiquidFuel:1","Oxidizer:1","MonoPropellant:1.5","Ore:0.2","XenonGas:10"}) {
 var part=new Part();var anim=new ModuleAnimateGeneric{part=part};var sw=new ModuleB9PartSwitch{part=part,volume=volume};var mod=new ModuleInflataFuelLock{part=part};part.Modules.Add(anim);part.Modules.Add(sw);part.Modules.Add(mod);
 foreach(string entry in spec.Split(',')){var pair=entry.Split(':');float rate=float.Parse(pair[1],System.Globalization.CultureInfo.InvariantCulture);sw.CurrentTankType.resources.Add(new TankResource{ResourceName=pair[0],unitsPerVolume=rate});part.Resources.Add(new PartResource{resourceName=pair[0],maxAmount=volume*rate});}
 var ec=new PartResource{resourceName="ElectricCharge",amount=10,maxAmount=10};part.Resources.Add(ec);
 mod.OnStart(PartModule.StartState.None);mod.OnUpdate();foreach(var d in sw.CurrentTankType.resources)Check(part.Resources.Get(d.ResourceName).maxAmount==0,"stowed capacity");Check(ec.amount==10 && ec.maxAmount==10,"ancillary resource changed");
 anim.animTime=1;mod.OnUpdate();foreach(var d in sw.CurrentTankType.resources)Check(Math.Abs(part.Resources.Get(d.ResourceName).maxAmount-volume*(double)d.unitsPerVolume)<0.001,"deployed capacity");
 var cargo=part.Resources[0];cargo.amount=4;int before=anim.toggles;mod.DeflateDepot();Check(anim.toggles==before,"loaded tank deflated");
 mod.storageUnlocked=false;anim.animTime=0;mod.OnUpdate();Check(cargo.amount==4 && mod.storageUnlocked,"save cargo lost");
 anim.animTime=1;mod.DeflateDepot();Check(anim.toggles==before,"saved loaded tank deflated");
 cargo.amount=cargo.maxAmount+5;mod.OnUpdate();Check(cargo.amount==cargo.maxAmount,"over-capacity cargo lost");
 cargo.amount=0;mod.DeflateDepot();Check(anim.toggles==before+1,"empty deflation blocked");Check(cargo.maxAmount==0,"capacity reopened on deflate frame");
 anim.animTime=0;anim.moving=false;mod.OnUpdate();anim.animTime=1;mod.OnUpdate();Check(cargo.maxAmount>0,"reinflation failed");
 cargo.amount=5;HighLogic.LoadedSceneIsFlight=false;HighLogic.LoadedSceneIsEditor=true;Call(mod,"Update");Check(cargo.amount==0,"editor not empty");Check(ec.amount==10,"editor ancillary resource changed");HighLogic.LoadedSceneIsFlight=true;HighLogic.LoadedSceneIsEditor=false;
 }
 Console.WriteLine("PASS "+count+" assertions: six tank configurations x three sizes; capacity, deflation guard, saved cargo, reinflation and editor empty rule. Mock API harness, not a KSP runtime test.");
}
}

