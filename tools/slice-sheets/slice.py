# Slicer das sheets do usuario -> game/Assets/Resources/Art/*
# Flood-fill do fundo cinza + trim. Botoes: reconstruidos sem texto (pontas + faixa limpa).
from PIL import Image, ImageDraw
import os

VAULT = "C:/Users/guilh/Documents/OPENCODE/vault/07-REFERENCIAS"
SRC = os.path.join(VAULT, "Gemini_Generated_Image_2c990z2c990z2c99.jpg")
ART = "C:/Users/guilh/Documents/OPENCODE/game/Assets/Resources/Art"
CONTACT = "C:/Users/guilh/AppData/Local/Temp/opencode/slice_contact.png"

# (outkey, subdir, x0,y0,x1,y1 frac) — rects generosos, trim ajusta
RECTS = [
    ("panel_blank", "UI", .349, .220, .497, .360),
    ("panel_med", "UI", .349, .357, .497, .477),
    ("panel_small", "UI", .501, .357, .540, .477),
    ("icon_coin", "Icons", .794, .557, .837, .638, 25),
    ("icon_food", "Icons", .849, .557, .900, .638, 25),
    ("icon_water", "Icons", .903, .557, .945, .638, 25),
    ("icon_metal", "Icons", .791, .756, .844, .855, 25),
    ("icon_medicine", "Icons", .794, .656, .839, .751, 25),
    ("icon_ship", "Icons", .895, .756, .951, .855, 25),
    ("anchor_steel", "Icons", .551, .326, .596, .410),
    ("anchor_gold", "Icons", .616, .326, .661, .410),
    ("anchor_dark", "Icons", .682, .326, .718, .410),
    ("isl_village", "Islands", .011, .553, .211, .799),
    ("isl_reef", "Islands", .218, .547, .375, .775),
    ("isl_castle", "Islands", .388, .503, .536, .747),
    ("isl_light", "Islands", .533, .516, .659, .747),
    ("isl_church", "Islands", .655, .514, .797, .747),
    ("ship_galleon", "Ships", .560, .800, .697, .977),
    ("serpent_1", "UI", .113, .807, .254, .938),
    ("serpent_2", "UI", .266, .807, .392, .938),
    ("serpent_3", "UI", .411, .807, .538, .938),
    ("serpent_4", "UI", .731, .807, .886, .938),
]

BTN_SRC = {
    "normal": (.541, .070, .667, .151),
    "highlight": (.676, .154, .783, .224),
    "pressed": (.791, .154, .900, .224),
    "menu": (.791, .232, .900, .303),
}

ISLAND_MAP = {
    "isl_village": "island_FishingVillage",
    "isl_light": "island_TradingPort",
    "isl_castle": "island_Capital",
    "isl_reef": "island_Uninhabited",
    "isl_church": "island_Dangerous",
}


def crop_frac(im, box):
    w, h = im.size
    return im.crop((int(box[0] * w), int(box[1] * h),
                    int(box[2] * w), int(box[3] * h))).convert("RGBA")


def dissolve_text_lines(im, lum_thresh=105):
    # dissolve linhas de texto (grupos de componentes escuros pequenos alinhados),
    # preserva detalhes isolados (janelas, pedras) e contornos longos
    w, h = im.size
    src = im.convert("RGB")
    px = src.load()
    dark = [[False] * w for _ in range(h)]
    for y in range(h):
        for x in range(w):
            r, g, b = px[x, y]
            if (r + g + b) // 3 < lum_thresh:
                dark[y][x] = True
    seen = [[False] * w for _ in range(h)]
    comps = []
    for y in range(h):
        for x in range(w):
            if not dark[y][x] or seen[y][x]:
                continue
            stack = [(x, y)]
            seen[y][x] = True
            xs = []
            while stack:
                cx, cy = stack.pop()
                xs.append((cx, cy))
                for dx in (-1, 0, 1):
                    for dy in (-1, 0, 1):
                        nx, ny = cx + dx, cy + dy
                        if 0 <= nx < w and 0 <= ny < h and dark[ny][nx] and not seen[ny][nx]:
                            seen[ny][nx] = True
                            stack.append((nx, ny))
            bw = max(p[0] for p in xs) - min(p[0] for p in xs) + 1
            bh = max(p[1] for p in xs) - min(p[1] for p in xs) + 1
            if bw < 50 and bh < 50 and len(xs) < 1500:
                comps.append(xs)
    comps.sort(key=lambda c: sum(p[1] for p in c) / len(c))
    rows = []
    for c in comps:
        cy = sum(p[1] for p in c) / len(c)
        placed = False
        for r in rows:
            if abs(r[0] - cy) < 14:
                r[1].append(c)
                placed = True
                break
        if not placed:
            rows.append([cy, [c]])
    out = im.copy()
    po = out.load()
    for _, group in rows:
        if len(group) < 2:
            continue
        for comp in group:
            for (x, y) in comp:
                sr = sg = sb = n = 0
                for dy in range(-4, 5):
                    for dx in range(-4, 5):
                        nx, ny = x + dx, y + dy
                        if 0 <= nx < w and 0 <= ny < h and not dark[ny][nx]:
                            r, g, b = px[nx, ny][:3]
                            sr += r
                            sg += g
                            sb += b
                            n += 1
                if n > 0:
                    po[x, y] = (sr // n, sg // n, sb // n, 255)
    return out


def clean(im, thresh=60):
    cw, ch = im.size
    px = im.load()
    def light(x, y):
        r, g, b = px[x, y][:3]
        return (r + g + b) // 3 > 150
    seeds = []
    for x in range(0, cw, 4):
        if light(x, 0): seeds.append((x, 0))
        if light(x, ch - 1): seeds.append((x, ch - 1))
    for y in range(0, ch, 4):
        if light(0, y): seeds.append((0, y))
        if light(cw - 1, y): seeds.append((cw - 1, y))
    if not seeds:
        seeds = [(0, 0), (cw - 1, 0), (0, ch - 1), (cw - 1, ch - 1)]
    for s in seeds:
        ImageDraw.floodfill(im, s, (0, 0, 0, 0), thresh=thresh)
    bbox = im.getbbox()
    if not bbox:
        return None
    x0, y0, x1, y1 = bbox
    pad = 3
    return im.crop((max(0, x0 - pad), max(0, y0 - pad),
                    min(cw, x1 + pad), min(ch, y1 + pad)))


def tint(im, mult, add=0):
    out = im.copy()
    px = out.load()
    for y in range(out.size[1]):
        for x in range(out.size[0]):
            r, g, b, a = px[x, y]
            px[x, y] = (min(255, int(r * mult + add)),
                        min(255, int(g * mult + add)),
                        min(255, int(b * mult + add)), a)
    return out


def avg_rgb(im):
    px = list(im.convert("RGB").getdata())
    n = len(px)
    return [sum(c[i] for c in px) / n for i in range(3)]


def despike_interior(im):
    # dissolve texto escuro (lum<105) preservando a borda externa (4px)
    w, h = im.size
    src = im.convert("RGB")
    px = src.load()
    dark = [[False] * w for _ in range(h)]
    for y in range(4, h - 4):
        for x in range(4, w - 4):
            r, g, b = px[x, y]
            if (r + g + b) // 3 < 105:
                dark[y][x] = True
    out = im.copy()
    po = out.load()
    for y in range(4, h - 4):
        for x in range(4, w - 4):
            if not dark[y][x]:
                continue
            sr = sg = sb = n = 0
            for dy in range(-3, 4):
                for dx in range(-3, 4):
                    if not dark[y + dy][x + dx]:
                        r, g, b = px[x + dx, y + dy][:3]
                        sr += r
                        sg += g
                        sb += b
                        n += 1
            if n > 0:
                po[x, y] = (sr // n, sg // n, sb // n, 255)
    return out


def rebuild_button(src, band):
    # pontas 20px (rolos, sem texto) + faixa limpa do painel; saida 160x64
    W, H = 160, 64
    left = src.crop((0, 0, 20, src.size[1])).resize((20, H))
    right = src.crop((src.size[0] - 20, 0, src.size[0], src.size[1])).resize((20, H))
    eb = avg_rgb(left.crop((0, H // 2 - 4, 20, H // 2 + 4)))
    bb = avg_rgb(band)
    mult = []
    for i in range(3):
        m = eb[i] / max(1, bb[i])
        if m < 0.7: m = 0.7
        if m > 1.3: m = 1.3
        mult.append(m)
    mid = tint(band, 1).resize((W - 40, H))
    px = mid.load()
    for y in range(mid.size[1]):
        for x in range(mid.size[0]):
            r, g, b, a = px[x, y]
            px[x, y] = (min(255, int(r * mult[0])), min(255, int(g * mult[1])),
                        min(255, int(b * mult[2])), a)
    out = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    out.paste(mid, (20, 0))
    out.paste(left, (0, 0), left)
    out.paste(right, (W - 20, 0), right)
    dr = ImageDraw.Draw(out)
    dr.rounded_rectangle([1, 1, W - 2, H - 2], radius=10, outline=(74, 58, 36, 255), width=2)
    return out


def save(img, subdir, name):
    d = os.path.join(ART, subdir)
    os.makedirs(d, exist_ok=True)
    img.save(os.path.join(d, name + ".png"))


def main():
    im = Image.open(SRC)
    thumbs = []

    def done(key, img):
        th = img.copy()
        th.thumbnail((120, 120))
        thumbs.append((key, th))

    for key, subdir, x0, y0, x1, y1, *rest in RECTS:
        th = rest[0] if rest else 60
        crop = crop_frac(im, (x0, y0, x1, y1))
        if key.startswith("panel_"):
            out = clean(crop, thresh=th)
        else:
            out = clean(dissolve_text_lines(crop), thresh=th)
        if out is None:
            print("VAZIO:", key)
            continue
        if key in ISLAND_MAP:
            base = ISLAND_MAP[key]
            save(out, "Islands", base + "_0")
            save(out.transpose(Image.FLIP_LEFT_RIGHT), "Islands", base + "_1")
            done(base, out)
        elif key == "ship_galleon":
            for v in ("ship_medium_0", "ship_medium_1"):
                save(out, "Ships", v)
            save(out.transpose(Image.FLIP_LEFT_RIGHT), "Ships", "ship_medium_1")
            done("ship", out)
        else:
            save(out, subdir, key)
            done(key, out)

    for name, box in BTN_SRC.items():
        src = clean(dissolve_text_lines(crop_frac(im, box), 80))
        if src is None:
            print("VAZIO BTN:", name)
            continue
        pw, ph = src.size
        band = clean(crop_frac(im, (.360, .240, .480, .270)))
        btn = rebuild_button(src, band)
        if name == "normal":
            save(btn, "UI", "btn_teal")
            done("btn_teal", btn)
        elif name == "highlight":
            save(btn, "UI", "btn_gold")
            save(tint(btn, 0.85), "UI", "btn_gold_pressed")
            done("btn_gold", btn)
        elif name == "pressed":
            save(btn, "UI", "btn_teal_pressed")
            done("btn_teal_pressed", btn)
        elif name == "menu":
            save(btn, "UI", "btn_teal_dark")
            save(tint(btn, 0.8), "UI", "btn_teal_dark_pressed")
            done("btn_teal_dark", btn)

    cols = 6
    cw, chh = 140, 160
    rows = (len(thumbs) + cols - 1) // cols
    sheet = Image.new("RGB", (cols * cw, rows * chh), (40, 40, 40))
    dr = ImageDraw.Draw(sheet)
    for i, (key, th) in enumerate(thumbs):
        x = (i % cols) * cw + 10
        y = (i // cols) * chh + 10
        sheet.paste(th, (x, y), th if th.mode == "RGBA" else None)
        dr.text((x, y + 130), key, fill=(255, 255, 255))
    sheet.save(CONTACT)
    print("OK", len(thumbs), "assets ->", ART)
    print("contact ->", CONTACT)


if __name__ == "__main__":
    main()
