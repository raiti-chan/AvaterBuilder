# AvaterBuilder

アバターのHierarchy構造、アニメーションコントローラー構造をテンプレート化し、着せ替えの簡易化、アニメーションコントローラーの生成をパターン化するツール群を提供する。

## Hierarchy構造

+ Avatar (AvatarDescripter)
    + Armature
    + Face
    + Body
    + [OtherBaseParts]
    + Hairs
        + Default
        + ...
    + Wears
        + Default
        + ...
    + Accessory
        + ...
    + Object
        + ...
    + AnchorOverride

- アバター直下に顔、体、他ベースパーツを配置。
- Hairs 髪群、リングメニューで切り替え化
- Wears 服群、リングメニューで切り替え化 
- Accessory 服に属さないアクセサリー群、個別にオンオフ可能
- Object 手とかに持たせるオブジェクト群、将来的に個別にオンオフできるようにしたい



## 必須コンポーネント
- WearSetup
- BoneRemapper
- AvatarBuilder
- RingMenuEditor
- AnimationControllerUtility
    - AnimationControllerSplitter
    - AnimationControllerMergeer
    - AnimationControllerParamClener

## ほしい機能
- BoneRemapperで、ボーンを子に配置する際に専用オブジェクトの中に格納する
- ビルドした際、別シーンにコピーを配置し、そちらにスケールを適用してアップロード
    - アップロードのバージョンがシーンにも保存されるためロールバックを可能にする。
    - ビルド時にスケールを適用することによってきっとなんかメリットある
