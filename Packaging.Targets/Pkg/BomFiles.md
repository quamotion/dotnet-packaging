# BOM File Format

## What's a BOM?

A BOM file is a Bill of Materials. You can think of it as a list of files installed by an installer.

## What's stored inside the BOM?

A BOM file contains a lot of whitespace. Really, lots of it.

A BOM file consists of:

* A header
* A section of variables
* A index, usually at the end of the file, which maps simple integers (1, 2, 3,...) to regions (offset + length) inside the file. The index seems to contain `0xaaa` entries.
* A list of 'available' pointers.
* A list of 'variables'. These variables map variable-length names to items in the index. So far, these variables are known
  * Paths
  * HLIndex
  * Size64
  * BomInfo
  * VIndex

## Variables

### Directory Structures

* A `BomPaths` object, which almost always occupies `0x1000` bytes (hence the huge amount of whitespace for small installers).
It contains a header (a `BomTree` object), followed by the list of all files in this directory structure.

The paths variable contains, you might have guessed it, the list of files embedded in the BOM. Each file is described by at least three separate objects:

* A `BomFile` object which contains the ID of the parent of this file, as well as the file name.
* A `BomPathInfo` object which assigned a unique ID to this file and references the `BomPathInfo2` object.
* A `BomPathInfo2` object which contains the actual metadata of the file - more or less the output of `stat`: last modified, owner, size,...

## Example

This source tree contains a sample macOS installer, `Sample.pkg`, which deploys a file `hello.txt` to `/Applications`. The BOM file embedded in this installer is available as `BOM`. 

The index of this BOM file contains the following items:

| Index  | Offset   | Length   | Type         | Description      |
| ------ | -------- | -------- | ---------    | ---------------- |
| `0x0`  | `0x0000` | `0x0000` | BOM Header   |                  |
| `0x1`  | `0x33D5` | `0x001C` | Variable     | BomInfo          |
| `0x2`  | `0x1236` | `0x0015` | Variable     | Paths            |
| `0x3`  | `0x0236` | `0x1000` | BomPaths     | Paths            |
| `0x4`  | `0x021C` | `0x0015` | Variable     | HLIndex          |
| `0x5`  | `0x1271` | `0x1000` | BomPaths     | HLIndex          |
| `0x6`  | `0x0200` | `0x000D` | Variable     | VIndex           |
| `0x7`  | `0x124B` | `0x0015` |              | VIndex           |
| `0x8`  | `0x22A2` | `0x0080` | BomPaths     | VIndex           |
| `0x9`  | `0x2271` | `0x0015` | Variable     | Size64           |
| `0xA`  | `0x235E` | `0x1000` | BomPaths     | Size64           |
| `0xB`  | `0x335E` | `0x001F` | BomPathInfo2 | `.`              |
| `0xC`  | `0x1260` | `0x0006` | BomFile      | `.`              |
| `0xD`  | `0x1266` | `0x0008` | BomPathInfo  | `.`              |
| `0xE`  | `0x337D` | `0x001F` | BomPathInfo2 | `Applications`   |
| `0xF`  | `0x2286` | `0x0011` | BomFile      | `Applications`   |
| `0x10` | `0x2297` | `0x0008` | BomPathInfo2 | `Applications`   |
| `0x11` | `0x339C` | `0x0023` | BomPathInfo  | `hello.txt`      |
| `0x12` | `0x33BF` | `0x000E` | BomFile      | `hello.txt`      |
| `0x13` | `0x33CD` | `0x0008` | BomPathInfo2 | `hello.txt`      |
