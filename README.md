# Pixel 11 Pro Fold (yogi) for ExpressiveOS

Device tree for Google Pixel 11 Pro Fold (`yogi`, Tensor G6 `malibu`, GKI
6.12), built from the stock factory image `CD1A.261005.003`.

It's the Pixel 11 Pro XL (`kodiak`) tree adapted for yogi. Stock yogi and
kodiak share the bootloader, radio, kernel command line, partition layout and
first-stage ramdisk. The fold itself is described by the stock vendor
(`device_state_configuration.xml`, `display_layout_configuration.xml`), but
the framework, SystemUI and Settings fold behaviour (device states, hinge
area, rear display, per-state rotation, split shade, large-screen letterboxing)
comes from Google's product RROs, which this ROM doesn't ship. Those values
are generated from the stock yogi overlays with `tools/rro2xml.py` into
`rro_overlays/*Yogi`. The fingerprint sensor is in the power button
(`TARGET_HAS_UDFPS := false`).

**Untested:** this tree has not been booted on a yogi yet.

## Layout

| Image | Source |
| --- | --- |
| system, system_ext, product | built (ExpressiveOS) |
| init_boot (generic ramdisk) | built |
| vendor_boot (first-stage fstab, recovery) | built |
| vbmeta, vbmeta_system, vbmeta_vendor | built (verification disabled: `--flags 3`) |
| boot (GKI kernel), dtbo | stock |
| pvmfw | built (AOSP) |
| vendor, vendor_dlkm, system_dlkm | stock |
| vendor_kernel_boot, bootloader/modem firmware | stock, flashed raw by the OTA |

Google publishes no malibu vendor sources (SELinux policy, HAL configs), so
vendor stays stock.

## Building

ExpressiveOS builds this tree with its build kit ([`expressiveos-cheetah/`](https://github.com/denson9874/Denson9874-Device-Central/tree/main/expressiveos-cheetah)),
in the source tree ExpressiveOS's own manifest syncs (`~/expressiveos`, set up by the kit's
`setup.sh`). The kit's `devices/yogi.conf` holds what's particular to this phone: the lunch
target `lineage_yogi-cp2a-user`, the model, the release folders, and where its kernel, vendor and
firmware come from.

```bash
EOS_DEVICE=yogi ~/expressiveos-cheetah/tools/eos-images.sh   # the build
EOS_DEVICE=yogi ~/expressiveos-cheetah/tools/eos-pack.sh     # the fastboot zip
```

From the runner, that's the action `expressiveos-build` with query `device=yogi`.

The manifest doesn't sync the two folders of Google's files this tree needs. The kit's
`prepare.sh` puts them in place before each build:

- `vendor/google/yogi-images`, whole stock partition images (vendor, vendor_dlkm,
  system_dlkm, boot, dtbo, firmware), made by this tree's `extract-images.sh` from the newest
  stock image zip on the build server (`~/android-dt/firmware/image-yogi-*.zip`, from
  [Google's factory image](https://developers.google.com/android/images)), again only when
  that zip changes. It isn't published.
- `vendor/google/yogi`, the proprietary blobs from `proprietary-files.txt`, made with
  `./extract-files.py <dumpyara dump of the yogi image>`. They aren't published; the build
  server keeps its own copy.

The kit builds the `user` variant. `userdebug` mounts debugfs early, which the stock vendor
policy denies, and the display driver can panic on boot.

`patches/` and `apply-patches.sh` come from before ExpressiveOS had its own sources; the kit
doesn't run them.

Extract the blobs from the phone's own image, never from kodiak's: their contents
match, but stock signs several of them with the phone's own platform key, and the
stock vendor SELinux policy only trusts that phone's key.
