# GitHub Copilot Instructions for Better Beggars (Continued)

## Mod Overview and Purpose

**Better Beggars (Continued)** is a mod for the game RimWorld, specifically designed to enhance the beggar interactions introduced in the Ideology DLC. The mod allows for greater customization of beggar events and interactions, providing a richer and more dynamic gameplay experience when dealing with this storyline. It updates and continues the work initially done by Fraser Hitchens.

## Key Features and Systems

- **Customizable Begging Amounts**: Adjust how much beggars request, with options from 0.1x to 2x the colony's wealth.
- **Item Request Customization**: Define what items beggars can request from your colony.
- **Delayed Rewards**: When helping beggars, there's a chance they will send you a reward later.
- **New Events**:
  - **Beggar Addicts**: These beggars request drugs to aid with their addictions, adding complexity and choice for players.
  - **Beggars Chased by Raiders**: Assist these beggars by offering them refuge for a few days in exchange for potential temporary labor or permanent benefits.

### Planned Features

- Expansion of the item list for beggars’ requests.
- Additional unique events involving beggars.

## Coding Patterns and Conventions

- **Coding Language**: The mod is written in C#. The .NET Framework version used is 4.8.
- **Class Naming Conventions**: Classes are named using the PascalCase convention. For instance, `QuestNode_Root_Beggars_Chased`.
- **File Organization**: Each logical component of the mod is separated into individual C# files, enabling modular design and maintenance.

### XML Integration

- The mod uses XML files for defining game data (e.g., items, events) that can be dynamically integrated into the game.
- XML Defs follow RimWorld's standard structure, and this allows for expanding or modifying existing content without altering base game files.

## Harmony Patching

- **Harmony** is used for patching the existing game methods to introduce new functionality or alter existing behavior.
- This ensures compatibility and minimizes conflicts, enhancing mod stability.
- When creating patches, ensure you:
  - Correctly identify the target methods or classes.
  - Apply pre-fix and post-fix methods appropriately to inject custom logic.

### Suggestions for Utilizing Copilot

- **Auto-Completion**: Copilot can assist with writing repetitive game definitions or C# boilerplate code needed for new events or interactions in RimWorld.
- **Error Handling**: Utilize Copilot to suggest and implement robust error handling, especially in Harmony patches, to catch and log exceptions without crashing the game.
- **Modular Code**: Leverage Copilot’s pattern recognition to suggest modular reusable code blocks for common functionalities like generating random events or item requests.
- **Documentation and Comments**: Use Copilot to help create comprehensive comments and documentation for your code, improving readability and future maintainability.

## FAQ

**Q: Can this mod be added to an existing save?**
- Yes, based on testing, it appears to be safe, though not guaranteed.

**Q: Can this mod be safely removed from a save?**
- Yes, it has been tested to safely remove from a save file.

## Compatibility

- The mod should be compatible with other mods unless they modify the vanilla beggar quest or item definitions.
- If you encounter compatibility issues, it is advised to enable this mod alone first to identify conflicts.

## Troubleshooting and Support

- **Error Logs**: Always utilize the Log Uploader for reporting issues.
- **Discord**: For best support, report issues through the designated Discord channel.
- **Solution Sharing**: If you discover a solution to a problem, contribute to the GitHub repository to assist other users.

## Quality Assurance

- Test mod alone to ensure base functionality works.
- Reintroduce other mods gradually to identify potential conflicts.

## Contribution and Licensing

- **License**: This mod is licensed under GNU General Public License v3.0.
- Contributions are welcome; please ensure compatibility and quality of new features through thorough testing.

### Tags

- Event, Customization

This document serves as a comprehensive guide for modders and contributors looking to enhance or utilize the Better Beggars (Continued) mod. Happy modding!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
