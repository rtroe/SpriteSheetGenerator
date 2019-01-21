# SpriteSheetGenerator
Another Sprite Sheet Generator. Searches a directory for all *.png files and packs them into a sprite sheet. It also generates a 'json' with locations and enum key list.

# Usage
Start the exe specifing the following arguments:
`-w [width] -h [height] -dir [path/to/files] -output(optional) [path/to/output/dir]`
* The default `Width` and `Height` is 256 currently,
* Default `dir` is the path the exe is in.
* The default output is `MyDocuments/Pictures/SpriteGen/[Date]/[time]/spritesheet.png]`

# Known Bugs
The packing algorythm can definitely be refined. Currently it sorts the icons by width. There can be some optimization by packing files deeper into lines that don't all have the same resolution.

