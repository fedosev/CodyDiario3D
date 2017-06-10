using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARFormOptions {



    [System.Serializable]
    public class GridTransparency {

		[Range(0f, 1f)]
        public float alphaValue;
        public Vec2 posForBorder;
        public Vec2 posForQuad;

        public GridTransparency(float alpha, Vec2 posForBorder, Vec2 posForQuad) {
            this.alphaValue = alpha;
            this.posForBorder = posForBorder;
            this.posForQuad = posForQuad;
        }

    }

	[CreateAssetMenuAttribute]
    public class GridTransparencies : ScriptableObject {

        public GridTransparency[] items = {
            new GridTransparency(
                0.2f,
                new Vec2(8, 14),
                new Vec2(8, 12)
            ),
            new GridTransparency(
                0.4f,
                new Vec2(9, 14),
                new Vec2(9, 12)
            ),
            new GridTransparency(
                0.6f,
                new Vec2(10, 14),
                new Vec2(10, 12)
            ),
            new GridTransparency(
                0.8f,
                new Vec2(11, 14),
                new Vec2(11, 12)
            ),
            new GridTransparency(
                1f,
                new Vec2(12, 14),
                new Vec2(12, 12)
            )
        };
    }

}