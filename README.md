# Glimpse #

Glimpse is a tiny tool which aims to bring the usefulness of Mac OS X's Quick Look feature to Windows.

It seamlessly integrates with Windows Explorer without adding any bloat to the system. It's not even a shell extension. Just hit win+space and it pops up.

## Supported files ##

- Image files (png, jpg, tiff, gif)
- Text files
- RTF files
- Folders and hard drives
- Everything supported by Explorer's preview pane like:
 - PDF files 
 - Word documents
 - Excel sheets
 - Power Point slides

## How does it work? ##

Currently you have to run an AutoHotkey script which listens for the win+space hotkey inside an Explorer window. If hit, the script launches a instance of Glimpse by passing the explorer's window handle. Glimpse then gets the path to the selected item in the active explorer window via a Shell COM API. So no hacky window hierarchy walking or any undocumented functionality is used to interop with Explorer.

## TODO ##

- Track Explorer's selection
- Open Glimpse from Desktop and Explorer's tree
- Video files
- Muliple files preview
- 'Smart' window size handling
- Remove message boxes