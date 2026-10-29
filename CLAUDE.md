# Taylor's Tales Uploader

## Current goal

Get the uploader working cleanly in Unity 6 (6000.4.1f1), tidy it up and polish it visually, so the owner can **record a portfolio video** of themselves using it to create book content.

The portfolio description, in the owner's words:

> The uploader is where the books are built, and it was entirely my design. I wrote custom Unity editor code that turns raw scene layouts, text and audio into finished asset bundles, along with the JSON metadata that tells the main project which bundle belongs to which page and where each paragraph goes.
>
> During development the asset bundles were written to persistent data, and at release they were served from Azure Blob Storage. The point was to separate concerns. Artists and writers work in the uploader, and the main project stays lean and only reads finished, packaged content. I'd still take the same approach now, moving the heavy lifting into a build time tool so the pipeline outlasts the project.

The video is recorded **in the editor, not in a build**. The pipeline relies on editor-only APIs (`PrefabUtility`, `AssetImporter`, `BuildPipeline.BuildAssetBundles`, `AssetDatabase`), so a player build can't do any of it, and the portfolio text calls it a build-time editor tool.

## How it works

- The editor windows live in `Assets/Editor/` and sit under the **Taylor's Tales** menu, in pipeline order:
  - `0` Book Pages
  - `1` Page Text
  - `2` Environment Uploader
  - `3` Fact Uploader
  - Tools: Word List, Sentence List, Bundle Target
  - Open Output Folder
  - Recording Layout
- Output goes to `%USERPROFILE%/AppData/LocalLow/DefaultCompany/TaylorsTalesAssets/ChickenAndTheFox/`:
  - `Pages/JSONPageData.json`: page texts per language (English, Irish, French, Spanish).
  - `Pages/page_<N>_environmentcanvas.unity3d`: the scene's canvases tagged `StaticEnvironmentCanvas` and `InteractionCanvas`, plus the skybox.
  - `Facts/Facts.json`, plus one GUID-named image bundle per fact.
- Inputs:
  - Source text: `Assets/StreamingAssets/PageTexts/<Language>.txt`.
  - Fact text: `Assets/StreamingAssets/Facts/Facts<Language>.txt`.
  - Fact images: `Assets/FactImages/<english trigger word>/`.
  - Page scenes: `Assets/Scenes/page N.unity`.
- Fact trigger words are marked in page text as `<color=red>word</color=red>`. The reader expects that exact closing tag.
- **Keep the JSON and bundle formats unchanged.** The main reader project (not in this repo) consumes them.
- Bundles target Android, falling back to the active platform (Windows) when the Android module isn't installed. It currently isn't installed.

## Done (2026-10-29)

- Fixed the Unity 6 compile errors in the TMP example scripts (`uvs0` is now `Vector4[]`).
- Bundle target now falls back from Android; it can be switched under Tools → Bundle Target.
- Bug fixes:
  - Page Text loaded the wrong page (it indexed by list position).
  - Fact Uploader threw GUI layout errors, skipped `.png` images, and crashed on missing folders.
  - Environment Uploader threw null references when a canvas or the skybox was missing, or the `Pages` folder didn't exist.
  - Word List had encoding damage.
  - The meta/manifest file filter used `||` where it needed `&&`.
- Deleted dead scripts: `Uploader`, `PageTextUploader`, `InteractionsUploader`, `InteractionCreator`, `assetb`, `Test`.
- Visual polish on every window: branded header, output and target info, progress bars, result dialogs, status rows, thumbnails, rich-text preview, trigger-word marking.
- `FactImages/clever/istockphoto-177408673-612x612.jpg` was a WebP named `.jpg`. It has been converted to a real JPEG.
- A full pipeline smoke test passed: page 11 built (6.3 MB), and 37 facts with their bundles uploaded. The test output was then cleared and the canvas prefabs and ExportFolder were reverted.

## Outstanding / next steps

1. **The system ran out of commit memory** (Windows commit limit, not Unity: Unity was using about 1.2 GB) during the Book Pages thumbnail capture at page 11. The owner is freeing memory. Check that the machine is healthy before resuming.
2. **New and untested:** `BookPagesWindow.cs`, which is the Book Pages grid plus the Recording Layout menu item. The Environment Uploader now auto-fills the page number from the open scene, and `TextPageUploader.OpenAtPage` was added. None of this has compiled since the memory failure. Confirm it compiles cleanly.
3. Finish the thumbnails. Thumbnails exist for TitlePage and pages 1–10 in `Library/TaylorsTales/Thumbnails/`. Run **Book Pages → Capture thumbnails**, which now unloads assets after each scene, then view the PNGs to confirm they look right (overlay canvases are switched to camera mode during capture).
4. Optional: install the Android module so the video shows release bundles.
5. Optional: save the Recording Layout under Window → Layouts → Save Layout.
6. Nothing has been committed. The diff includes many image `.meta` changes, which are Unity 6 reserializing the importers and are expected.
7. **Done (2026-10-29):** the portfolio description has been rewritten in `PortfolioWithReact/public/projectsInfo.json` (id 6, `introText[1]` and `infoText[1]`). It covers an intro plus four sections: Page Text, Environment, Facts, and Word and Sentence libraries. The video is `public/videos/candfUploader.mp4`. The original reminder follows. **Reminder: when everything above is finished, write an updated portfolio description** to replace the one quoted under "Current goal". Write it in the owner's first-person voice and keep the separation-of-concerns point. Reflect what the uploader can now do:
   - The Book Pages overview.
   - Language-aware page text with fact trigger-word marking.
   - The scene checks before building a bundle.
   - The fact image bundles.
   - The Unity 6 upgrade.
   - The configurable bundle target.

   Only describe features that have actually been verified as working.

## Testing tips

- Unity is usually open on this project, so batch mode can't run. To exercise a window, drop a temporary `[InitializeOnLoad]` editor script that sets `BaseEditorWindow.SuppressDialogs = true`, invokes the window methods via reflection, writes a report to a file, and then deletes itself. Focus Unity to trigger the compile.
- Afterwards, revert the side effects of a test build: `Assets/Prefabs/EnvironmentCanvas/`, `Assets/Prefabs/InteractionCanvas/` and `ExportFolder/`.
