# Pipeline de catalogo procedural: manifesto + contact sheets + classificacao.
# NAO toca no gerador. Uso: py -3.12 catalog.py
from PIL import Image, ImageDraw
import os
import glob
import json

V = 'C:/Users/guilh/Documents/OPENCODE/vault/07-REFERENCIAS/Assets'
OUT = 'C:/Users/guilh/Documents/OPENCODE/game/Assets/Procedural/TileCatalog'
os.makedirs(OUT, exist_ok=True)

PACKS = [
    ('MonochromePirates', V + '/monochrome-pirates/Default/Tiles'),
    ('MonochromeRPG', V + '/monochrome-rpg/Default/Tiles'),
]

BG = (181, 176, 221)


def is_bg(p):
    r, g, b = p[:3]
    return abs(r - BG[0]) + abs(g - BG[1]) + abs(b - BG[2]) < 60


def analyze(path):
    im = Image.open(path).convert('RGBA')
    w, h = im.size
    px = list(im.getdata())
    content = [p for p in px if p[3] > 128 and not is_bg(p[:3])]
    rot = {}
    for deg in (90, 180, 270):
        r2 = im.rotate(deg, expand=False)
        a = list(im.getdata())
        b = list(r2.getdata())
        rot[deg] = sum(1 for x, y in zip(a, b) if x != y)
    return {'w': w, 'h': h, 'content': len(content), 'rot90': rot[90],
            'rot180': rot[180], 'rot270': rot[270]}


# ---- allowlist verificada (uso em producao + zoom visual) ----
WATER = {'MonochromePirates': [6, 7, 8]}
GRASS_FILL = [0]  # rpg 0000: planicie com speckle, ver contact_sheet_rpg
SAND_EDGES = {  # (tile, sockets N/E/S/W) leitura do zoom t_edges
    3: ('LAND', 'LAND', 'WATER', 'WATER'),   # canto externo NW
    4: ('LAND', 'LAND', 'WATER', 'LAND'),    # borda N? ver zoom
    5: ('LAND', 'WATER', 'WATER', 'LAND'),   # canto externo NE
    20: ('LAND', 'LAND', 'LAND', 'WATER'),   # borda W?
    22: ('LAND', 'WATER', 'LAND', 'LAND'),   # borda E?
    37: ('WATER', 'LAND', 'LAND', 'WATER'),  # canto externo SW
    38: ('WATER', 'LAND', 'LAND', 'LAND'),   # borda S?
    39: ('WATER', 'WATER', 'LAND', 'LAND'),  # canto externo SE
}
SAND_FILL = [17, 21]
PROPS = {  # tile: (categoria, papel) - funcao evidente no zoom
    'MonochromePirates': {
        9: ('object', 'dock'), 86: ('object', 'anchor'),
        102: ('object', 'barrel'), 103: ('object', 'chest'),
        104: ('decoration', 'palm'), 107: ('object', 'bottle'),
        119: ('object', 'fish'), 121: ('decoration', 'rock'),
        122: ('landmark', 'skull'), 123: ('object', 'sword'),
        52: ('decoration', 'hill'), 94: ('object', 'planks'),
        120: ('object', 'sail'),
    },
    'MonochromeRPG': {
        13: ('decoration', 'tree'), 31: ('decoration', 'palm2'),
        33: ('building', 'castle'), 48: ('building', 'house'),
        49: ('building', 'hut'), 52: ('landmark', 'cave'),
        64: ('object', 'cross'), 65: ('object', 'cross2'),
        66: ('object', 'tombstone'),
    },
}
CHARS_P = [124, 125, 126, 127]
CHARS_G = [119, 120, 121, 122, 123, 124, 125, 126]
MODULE_RANGES = [  # suspeitos multi-tile (partes de navio etc.)
    ('MonochromePirates', 14, 16), ('MonochromePirates', 31, 33),
    ('MonochromePirates', 43, 50), ('MonochromePirates', 60, 67),
    ('MonochromePirates', 94, 101), ('MonochromePirates', 111, 118),
    ('MonochromePirates', 128, 135),
]


def in_ranges(pack, idx):
    for p, a, b in MODULE_RANGES:
        if p == pack and a <= idx <= b:
            return True
    return False


manifest, catalog = [], []
sw = {'pir': {}, 'rpg': {}}
for pack, tdir in PACKS:
    files = sorted(glob.glob(os.path.join(tdir, 'tile_*.png')))
    for f in files:
        idx = int(os.path.basename(f)[5:9])
        an = analyze(f)
        tid = ('pirates_tile_%04d' % idx) if pack == 'MonochromePirates' else ('rpg_tile_%04d' % idx)
        proj = 'game/Assets/Resources/Art/kenney/tiles/%s.png' % os.path.basename(f)[5:9]
        manifest.append({
            'id': tid, 'source_pack': pack,
            'source_path': 'vault/07-REFERENCIAS/Assets/%s/Default/Tiles/%s' % (
                'monochrome-pirates' if pack == 'MonochromePirates' else 'monochrome-rpg',
                os.path.basename(f)),
            'project_path': proj if os.path.exists(
                'C:/Users/guilh/Documents/OPENCODE/' + proj) else None,
            'width': an['w'], 'height': an['h'],
        })
        e = {'id': tid, 'source_pack': pack,
             'path': 'vault/07-REFERENCIAS/Assets/%s/Default/Tiles/%s' % (
                 'monochrome-pirates' if pack == 'MonochromePirates' else 'monochrome-rpg',
                 os.path.basename(f)),
             'category': 'unknown', 'role': 'unclassified', 'tags': [],
             'rotation_allowed': False, 'allowed_rotations': [0],
             'sockets': {'north': 'UNKNOWN', 'east': 'UNKNOWN',
                         'south': 'UNKNOWN', 'west': 'UNKNOWN'},
             'connections': {'north': False, 'east': False,
                             'south': False, 'west': False},
             'weight': 1, 'needs_review': True, 'confidence': 0.0,
             'module_size': [1, 1], 'module_candidate': False}
        if an['content'] == 0:
            e.update(category='empty', role='none', tags=['empty'],
                     confidence=1.0, needs_review=False, weight=0)
        elif pack == 'MonochromePirates' and idx in WATER.get(pack, []):
            e.update(category='water', role='open_water', tags=['water', 'sea'],
                     rotation_allowed=True, allowed_rotations=[0, 90, 180, 270],
                     sockets={k: 'WATER' for k in ('north', 'east', 'south', 'west')},
                     confidence=0.8, needs_review=False, weight=10)
        elif pack == 'MonochromeRPG' and idx in GRASS_FILL:
            e.update(category='terrain', role='grass_fill', tags=['grass'],
                     sockets={k: 'LAND' for k in ('north', 'east', 'south', 'west')},
                     connections={k: True for k in ('north', 'east', 'south', 'west')},
                     confidence=0.5, needs_review=True, weight=6)
        elif pack == 'MonochromePirates' and idx in SAND_EDGES:
            n, ee, s, w = SAND_EDGES[idx]
            e.update(category='coast', role='sand_edge', tags=['sand', 'coast', 'edge'],
                     sockets={'north': n, 'east': ee, 'south': s, 'west': w},
                     connections={'north': n == 'LAND', 'east': ee == 'LAND',
                                  'south': s == 'LAND', 'west': w == 'LAND'},
                     confidence=0.5, needs_review=True, weight=6)
        elif pack == 'MonochromePirates' and idx in SAND_FILL:
            e.update(category='terrain', role='sand_fill', tags=['sand'],
                     sockets={k: 'LAND' for k in ('north', 'east', 'south', 'west')},
                     connections={k: True for k in ('north', 'east', 'south', 'west')},
                     confidence=0.6, needs_review=True, weight=6)
        elif pack in PROPS and idx in PROPS[pack]:
            cat, role = PROPS[pack][idx]
            e.update(category=cat, role=role, tags=[role], confidence=0.6,
                     needs_review=True, weight=2)
        elif ((pack == 'MonochromePirates' and idx in CHARS_P)
              or (pack == 'MonochromeRPG' and idx in CHARS_G)):
            e.update(category='character', role='figure', tags=['character'],
                     confidence=0.6, needs_review=True, weight=0)
        if in_ranges(pack, idx) and e['category'] in ('unknown', 'object', 'decoration'):
            e['module_candidate'] = True
            e['tags'] = e['tags'] + ['suspect-module-part']
        if an['content'] > 0 and e['rotation_allowed'] is False and e['category'] in ('water', 'terrain'):
            if an['rot90'] == 0 and an['rot180'] == 0 and an['rot270'] == 0:
                e['rotation_allowed'] = True
                e['allowed_rotations'] = [0, 90, 180, 270]
        catalog.append(e)

with open(os.path.join(OUT, 'asset_manifest.json'), 'w') as f:
    json.dump({'sources': [
        'vault/07-REFERENCIAS/Assets/monochrome-pirates/Default/Tilesheet.txt',
        'vault/07-REFERENCIAS/Assets/monochrome-rpg/Default/Tilesheet.txt',
        'vault/07-REFERENCIAS/Assets/monochrome-pirates/Default/Tilemap/tilemap.png',
        'vault/07-REFERENCIAS/Assets/monochrome-rpg/Default/Tilemap/tilemap.png'],
        'tiles': manifest}, f, indent=1)
with open(os.path.join(OUT, 'tile_catalog.json'), 'w') as f:
    json.dump(catalog, f, indent=1)


def sheet(pack, tdir, outname):
    files = sorted(glob.glob(os.path.join(tdir, 'tile_*.png')))
    cols = 17
    rows = (len(files) + cols - 1) // cols
    tw = th = 16
    scale = 3
    cw, chh = tw * scale + 4, th * scale + 20
    sh = Image.new('RGB', (cols * cw, rows * chh), (20, 20, 20))
    dr = ImageDraw.Draw(sh)
    for i, fp in enumerate(files):
        im = Image.open(fp).convert('RGBA')
        bg = Image.new('RGBA', im.size, (150, 150, 150, 255))
        bg.paste(im, (0, 0), im)
        x = (i % cols) * cw + 2
        y = (i // cols) * chh + 2
        sh.paste(bg.resize((tw * scale, th * scale), Image.NEAREST), (x, y))
        dr.text((x, y + th * scale + 2),
                '%s %s' % (pack, os.path.basename(fp)[5:9]), fill=(255, 255, 0))
    sh.save(os.path.join(OUT, outname))
    print(outname, len(files))


from PIL import ImageDraw  # noqa
sheet('PIR', V + '/monochrome-pirates/Default/Tiles', 'contact_sheet_pirates.png')
sheet('RPG', V + '/monochrome-rpg/Default/Tiles', 'contact_sheet_rpg.png')
print('manifest:', len(manifest), 'catalog:', len(catalog))
