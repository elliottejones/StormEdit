# StormEdit
A Lua script editor for Stormworks: Build and Rescue


# What is this?

I find tuning and programming Lua scripts in Stormworks way too annoying. To edit a script, you must click through three different menus that all take time to load.

The idea behind this project, is to bypass the in-built Stormworks lua editor block entirely and edit externally.

This will be done though linking a Lua block to this app, and a reference to it being saved. Then the program will inject the Lua code you write in this app, directly into your Stormworks creation so you can spawn it.

Using an external code editor carries many other benefits to customisation and efficiency, which can be added at a later date.


# What is this built with?

StormEdit's GUI is built with C# and AvaloniaUI with it's MVVM (Model-View-Viewmodel) design pattern.

The built in code editor is AvalonEdit with the TextMate Lua syntax highlighting.

The rest of the logic is C#.

I hate installing things, so I made those stack choices to allow this to be built as a standalone executable.

The Lua minifier is a slightly adapted version of this amazing CLI tool:
https://github.com/stravant/lua-minify

