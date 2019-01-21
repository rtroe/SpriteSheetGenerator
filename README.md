# SpriteSheetGenerator
Another Sprite Sheet Generator. Searches a directory for all .png files and packs them into a sprite sheet. It also generates a 'json' file with locations and sizes of the textures within the sprite sheet. This json's information can be retrieved by using the generated C# enum key list.

# Usage
Start the exe specifing the following arguments:
`-w [width] -h [height] -dir [path/to/files] -output(optional) [path/to/output/dir]`
* The default `Width` and `Height` is 256 currently,
* Default `dir` is the path the exe is in.
* The default output is `MyDocuments/Pictures/SpriteGen/[Date]/[time]/spritesheet.png]`

# Known Bugs
The packing algorythm can definitely be refined. Currently it sorts the icons by width. There can be some optimization by packing files deeper into lines that don't all have the same resolution.

# Credits
The icon is from 'https://www.fatcow.com/free-icons'. These are great.
