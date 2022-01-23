// BetterBeggars.Harmony_PatchAll
using System.Reflection;
using HarmonyLib;
using Verse;

[StaticConstructorOnStartup]
public static class Harmony_PatchAll
{
	static Harmony_PatchAll()
	{
		Harmony val = new Harmony("better.beggars");
		val.PatchAll(Assembly.GetExecutingAssembly());
	}
}