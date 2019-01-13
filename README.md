# HMD
An Smod plugin that adds a custom weapon (originally) called the Heavy Mass Driver (H.M.D), but also known as the Ballistic HID.  
Uses ItemManager ID of 102.

# Installation
**[Smod2](https://github.com/Grover-c13/Smod2) and [ItemManager](https://github.com/probe4aiur/ItemManager) must be installed for this to work.**

Place the "HMD.dll" file in your sm_plugins folder.

# Configs
| Config        | Value Type | Default | Description |
| :-------------: | :---------: | :---------: |:------ |
| hmd_doubledrop_time | Float | 0.25f | Amount of time that a user has to double right-click the item in their inventory to toggle overcharge instead of dropping onto the ground |
| hmd_role_spawns | Integer List | Empty | The roles which have the desired spawnpoints of the HMD. |
| hmd_item_spawns | Integer List | 8 (Commander Keycard) | The items which have the desired spawnpoints of the HMD. |
| hmd_body_damage | Float | 80 | Amount of damage per shot on body. |
| hmd_head_damage | Float | 105 | Amount of damage per shot on head. |
| hmd_leg_damage | Float | 60 | Amount of damage per shot on leg. |
| hmd_106_damage | Float | 12 | Amount of damage per shot on 106. |
| hmd_tag_damage | Float | 10 | Amount of damage per shot when overcharged. |
| hmd_fire_rate | Float | 2 | Time (in seconds) between each shot. |
| hmd_magazine | Integer | 5 | Number of shots per magazine. |
| hmd_reserve_ammo | Integer | 15 | How much HMD ammo each player gets. |
| hmd_krakatoa | Integer | 15 | The amount of additional shoot sounds to play when shot. |
| hmd_suppressed_krakatoa | Integer | 7 | The amount of additional shoot sounds to play when shot with a suppressor. |
| hmd_overchargeable | Boolean | True | Whether or not the HMD can be overcharged. |
| hmd_overcharge_radius | Float | 15 | Radius of the overcharge effect from the overcharge device in in-game meters. |
| hmd_overcharge_damage | Float | 30 | Damage to be applied by the overcharge effect. |
| hmd_overcharge_glitch | Boolean | True | Whether or not to apply the glitch (nuke detonation) visual effect to people affected by the overcharge. |
| hmd_tag_time | Float | 5 | Time (in seconds) between an overcharged shot hitting a player and overcharge device detonation. |
| hmd_tag_glitches | Integer | 2 | Additional glitch effects to play for the tagged player when the overcharge device detonates. |
| hmd_chaos_count | Integer | 0 | Number of HMDs to be distributed within a Chaos Insurgency respawn. |
| hmd_mtf_count | Integer | 1 | Number of HMDs to be distributed within an MTF respawn. |
| hmd_mtf_ranks | Integer List | 12 (MTF Commander) | Roles within MTF's ranks that should have the HMDs distributed within. |
