# Copilot Instructions for Better Beggars (Continued) Mod

## Mod Overview and Purpose
**Mod Name**: Better Beggars (Continued)  
**Description**: This mod is an updated continuation of Fraser Hitchen's Better Beggars, designed to enhance the beggar mechanic introduced in the Ideology DLC for RimWorld. It extends and customizes interactions with beggars, making their appearances more engaging and their quests more rewarding.

## Key Features and Systems
- **Adjustable Beggar Requests**: Modify how much resources beggars request, with a configurable multiplier ranging from 0.1x to 2x of your colony's wealth.
- **Item Customization**: Change the type of items that beggars can request, offering more variety than the vanilla setup.
- **Delayed Rewards**: Beggars have a chance to send a reward later if you choose to help them, adding an element of delayed gratification.
- **New Events**:
  - **Beggar Addicts**: A scenario where beggars come asking for drugs to manage their addictions.
  - **Raid-Chased Beggars**: Beggars being chased by raiders who will join your colony temporarily if you choose to assist them.

## Coding Patterns and Conventions
- **Consistent Naming**: Use PascalCase for types and members in C#. XML tags follow standard RimWorld definitions and naming conventions.
- **Modular Code**: Keep code organized by functionality, as seen in separate C# files for settings, quests, and event handling.
- **Commenting and Documentation**: Comment critical sections of code to aid understanding, especially in complex methods like `RunInt` and `TestRunInt`.

## XML Integration
- **Event Definitions**: XML files define new incidents and quest scripts. Use `IncidentDef` and `QuestScriptDef` tags to create and manage new events.
- **Modifying Incidents**: Individual files (`Incidents_Map_Special.xml`, `Script_Beggars_DelayedReward.xml`, etc.) encapsulate specific events or modifications, facilitating easier updates and maintenance.

## Harmony Patching
- **Harmony**: The mod depends on the Brrainz Harmony library to patch RimWorld’s core methods, allowing modifications without altering the base game code.
- **Application**: Carefully apply patches to prevent conflicts. Always check for existing patches that might intersect with Better Beggars’ modifications.

## Suggestions for Copilot
To enhance productivity and code consistency, Copilot can be utilized in the following ways:

- **Generate Boilerplate Code**: Use Copilot to quickly scaffold class structures (e.g., settings, quests, events) and member signatures.
- **Suggest XML Stubs**: Automatically generate XML definitions for new quest scripts, ensuring they align with existing RimWorld schemas.
- **Standardize Harmony Patching**: Offer examples and guidance for writing robust Harmony patch classes and methods.
- **Simplify Debugging**: Assist in writing diagnostic and logging code, aiding in swift identification of mod errors or conflicts.

By leveraging these capabilities, Copilot can significantly accelerate the development process for the Better Beggars (Continued) mod while maintaining high-quality code standards.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

