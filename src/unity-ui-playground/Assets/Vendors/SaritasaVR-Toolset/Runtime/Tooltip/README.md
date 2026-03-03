# Tooltips

## Description

These tooltips support various animation types, e.g. fade, dithered fade, circle fill, masked dissolve, and scale. They are implemented with signed distance fields (SDF), allowing dynamic changes to their border and shape roundness. They also support other features like tooltip elements custom colors, transform offsets, look-at behaviors.

<img src=https://wiki.saritasa.rocks/assets/images/unity-tooltips-preview-3440e7121195814e650584352d6cb316.gif width="550px" height="400px" />

Almost every aspect of this tooltips updates in the editor so you can see the changes right away.
For example updating the position of the tooltip - updates line position. Updating animation alpha value - plays animation. Updating modifiers - immediately modifies the tooltip appearance.

<img src=https://wiki.saritasa.rocks/assets/images/unity-tooltips-editor-preview-8e9201d7e69dbfad773646a9d2072a05.gif width="750px" height="500px" />

:::note
All of these tooltips can be standalone (Used in worldspace without the controllers).
:::

## Usage

Animated tooltips based on the Animated Tooltip component that uses a component inherited from the Tooltip Animation script for animation. The Animated Tooltip component is the tooltip itself, requiring animations for fade-in and fade-out. You can create your animation scripts by extending Tooltip Animation script or use existing ones.
You can also extend other classes in the hierarchy if you don't need animations: TooltipBase -> TooltipWithLines -> AnimatedTooltips.

### Animations

There are some animations available out of the box:

- Scale; <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-scale-16c1718c9da3674b98598bd6c783dda1.gif width="550px" height="350px" />
<br/> <br/>

- Circle Fill; <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-circle-fill-e1d8be0f8da56d4fabaac23b05c88bf8.gif width="550px" height="250px" />
<br/> <br/>

- Dissolve; <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-dissolve-eede0bbccd165260b2b06037b04cbc12.gif width="550px" height="300px" />
<br/> <br/>

- Dithered Fade; <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-dither-5741dfc1178856af4c981eda5c0c35cd.gif width="550px" height="550px" />
<br/> <br/>

- Fade. <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-fade-ba820c035b41a6f38b1437dc580bdb05.gif width="550px" height="320px" />

### Modifiers

Tooltips support different kinds of modifier components that influence their behavior or visuals. You can add these components to your tooltip game object to extend functionality.

- TooltipBorder - allows setting up the border; <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-border-6beb050c97498e04f91931cb83d7369d.gif width="800px" height="500px" />
<br/> <br/>

- TooltipLineColors - allows controlling line and point colors; <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-lines-color-4998e4a0dcda712252c3effdb72163e2.gif width="800px" height="500px" />
<br/> <br/>

- TooltipRoundness - allows controlling the roundness of the tooltip edges; <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-roundness-62668fd4d735cf7dbe7a90b89356cc34.gif width="800px" height="500px" />
<br/> <br/>

- TooltipSizes - exposes settings for offsetting sizes and positions of tooltip elements; <br/>
<img src=https://wiki.saritasa.rocks/assets/images/unity-sizes-04a200f847f06dbae1024ae1da339c41.gif width="800px" height="500px" />
<br/> <br/>

- TooltipModifier - base class which you can extend to create your modifiers.

Different look-at behaviors bring rotation logic: <br/>

- XAxisOnly - Rotates only around the local X Axis; <br/>
- YAxisOnly - Rotates only around the local Y axis; <br/>
- XYAxis - Rotates around the local X and Y axes; <br/>
- XYAxisSmooth - Smoothly rotates around the local X and Y axes; <br/>
- LookAtBehavior - base class which you can extend to create your look-at logic.

:::note
You can find examples in the scene "**[005 - Tooltips] Tooltips Variations.**"
:::

## XR Controller with tooltips

Controllers Tooltips can be shown with `VR_ControllerTooltipManager.cs` manually or using Tutorial System.

Manager has public methods:

- SetTooltipText - sets text to tooltip; <br/>
- ShowTooltip - shows controller's tooltip with fade duration; <br/>
- HideTooltip - hides controller's tooltip with fade duration; <br/>
- ShowAllTooltips - shows all controller's tooltips with fade duration; <br/>
- HideAllTooltips - hides all controller's tooltips with fade duration.

## Optimization

Tooltips are divided into two groups Opaque and Transparent. Opaque are more efficient in general. If you want to save rendering time it is recommended to use Opaque ones.

### Opaque

- Dissolve, Circle Fill, and Scale cost **3 batches**. <br/>
- Dithering Fade costs **5 batches**. <br/>

### Transparent

- Fade costs **3 batches** but might be more while fading. See issue number 4.

Tooltips of the same type are batched together. For example: if you use only Circle Fill tooltips with the same stencil value (See issue 1), it will cost you **3 batches** for all tooltips.

## Known issues

### 1 Stencil problem

All tooltips and their text have a stencil value. By default, it is "8". Text with a specific value (8 in our case) can only be rendered on top of a tooltip's background with the same value. So, there is a problem when one tooltip overlaps the other and plays the fade-out animation; you can see the text of the tooltip that is fading out on top of other backgrounds because they have the same stencil value.
If this is the case and you want to avoid it, you should create another material for tooltip elements and text and assign a different stencil value.
After that, the number of batches will be twice as big because materials with different stencil values cannot be batched together. For example, if you have two controllers and tooltips that share the same material and the same stencil value, it should be 3 batches for all tooltips.
If you change the material of tooltips for the left controller to prevent overlapping, it will be 3 batches for left-hand tooltips + 3 batches for right-hand tooltips, so 6 batches for tooltips.

Assigning the material to the text and dissolving it with the tooltip might resolve this issue, but I haven't tried it yet. This problem doesn't exist on Scale tooltip type.

### 2 Depth problem

Opaque tooltips, by default, have a render queue value of "2000" and Z test value of "Less" in their materials. It means that they are **NOT** rendered on top of the geometry. It was made to prevent conflicts of rendering between points, lines, and backgrounds of different colors. If you need to see these tooltips through geometry, change the render queue to "3000" and Z test to "Always" in the tooltip materials.

Transparent tooltips are the opposite. They are rendered through the geometry by default.

### 3 Scale tooltip problem

Tooltip with scale animation behaves differently from other tooltips. Its size, borders, and roundness logic are implemented differently to maintain proportions during the scale.

### 4 Fade tooltip type

During rework, I removed the implementation that was interpolating vertex colors to change tooltip color. I think it might not be a really good decision. It's better to revert it back and see if batching improves.
