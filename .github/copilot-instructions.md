# GitHub Copilot Instructions for Better Beggars (Continued) Mod

## Mod Overview and Purpose

The Better Beggars (Continued) mod is an enhancement of the original Better Beggars mod by Fraser Hitchens, designed for the popular game RimWorld, specifically extending the functionalities introduced with the Ideology DLC. This mod refines the interactions and events surrounding beggars that appear in the game, adding depth and variety, and allowing players to customize these interactions more than the base game allows.

## Key Features and Systems

- **Customizable Begging Requests:** Adjust the multiplier for how much beggars will ask for, ranging from a 0.1x to a 2x multiplier based on your colony's wealth. The vanilla setting uses a 0.85x multiplier.
- **Item Request Flexibility:** Change the types of items beggars request, offering variety and customized gameplay experiences.
- **Delayed Rewards:** A chance for beggars to send a reward at a later time if you assist them, adding long-term consequences to your decisions.
- **New Beggar Events:**
  - **Beggar Addicts:** Events where beggars request drugs to deal with their addictions.
  - **Chased Beggars:** Beggars being pursued by raiders may seek your help, potentially joining your colony temporarily as they seek refuge.

### Planned Features

- Expanding the range of items beggars can request.
- Introducing more diverse beggar-related events.

## Coding Patterns and Conventions

This mod follows standard C# coding practices suitable for RimWorld modding:

- **Class Definitions:** Public and internal classes are organized for modular functionality across the various aspects of beggar events and interactions.
- **Method Declarations:** Methods are typically public for external interaction or private for internal logic intricate to the functioning of specific features.

## XML Integration

The mod uses XML to define various data-driven aspects, such as item lists and event parameters. XML integration allows for straightforward adjustments to in-game definitions, enhancing mod flexibility and user customization.

## Harmony Patching

The Harmony library is utilized to patch existing game behavior without directly modifying base game code, maintaining compatibility with other mods and updates. Here's how it facilitates the mod:

- **Event Modification:** By altering specific events using Harmony patches, the mod ensures compatibility while extending existing game features.
- **Behavior Tweaking:** Adjusts internal game mechanics to introduce new features or balance existing ones based on mod parameters.

## Suggestions for GitHub Copilot

To effectively assist with development:

1. **Generate Common Patterns:** Suggest class and method skeletons typical for RimWorld modding, including frequently used patterns like `QuestNode` extensions and `ModSettings`.
2. **Assist with XML Serialization:** Provide guidance on crafting XML templates for defining item lists and quest parameters.
3. **Harmony Patch Recommendations:** Suggest basic patch templates for integrating new events without conflict.
4. **Context-Aware Refactorings:** Offer suggestions based on detected patterns, i.e., suggest when to create new classes versus extending existing ones.
5. **Document Examples with Annotations:** Help annotate code with comments that improve readability and explain complex logic.

---
This file provides guidance on extending and maintaining the Better Beggars (Continued) mod, outlining the mod framework and areas GitHub Copilot can assist in enhancing the codebase for developers.
