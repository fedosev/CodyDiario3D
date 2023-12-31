﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARFormOptions {


    [System.Serializable]
    public struct Vec2 {
        public int x;
        public int y;
        public Vec2(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }


    [System.Serializable]
    public class GridColor {

        public string name;
        public Color color;
        public Vec2 posForBorder;
        public Vec2 posForQuad;

        public GridColor(string name, Color color, Vec2 posForBorder, Vec2 posForQuad) {
            this.name = name;
            this.color = color;
            this.posForBorder = posForBorder;
            this.posForQuad = posForQuad;
        }

    }


    public class GridColors : ScriptableObject {

        public GridColor[] items = {
            new GridColor(
                "Dark red",
                new Color(152f / 255f, 0f, 0f),
                new Vec2(1, 1),
                new Vec2(1, 3)
            ),
            new GridColor(
                "Red",
                new Color(1f, 0f, 0f),
                new Vec2(2, 1),
                new Vec2(2, 3)
            ),
            new GridColor(
                "Orange",
                new Color(1f, 153f / 255f, 0f),
                new Vec2(3, 1),
                new Vec2(3, 3)
            ),
            new GridColor(
                "Yellow",
                new Color(1f, 1f, 0f),
                new Vec2(4, 1),
                new Vec2(4, 3)
            ),
            new GridColor(
                "Green",
                new Color(0f, 1f, 0f),
                new Vec2(5, 1),
                new Vec2(5, 3)
            ),
            new GridColor(
                "Light blue",
                new Color(0f, 1f, 1f),
                new Vec2(6, 1),
                new Vec2(6, 3)
            ),
            new GridColor(
                "Nice blue",
                new Color(74f / 255f, 134f /255f, 232f / 255f),
                new Vec2(7, 1),
                new Vec2(7, 3)
            ),
            new GridColor(
                "Blue",
                new Color(0f, 0f, 1f),
                new Vec2(8, 1),
                new Vec2(8, 3)
            ),
            new GridColor(
                "Purple",
                new Color(153f / 255f, 0f, 1f),
                new Vec2(9, 1),
                new Vec2(9, 3)
            ),
            new GridColor(
                "Pink",
                new Color(1f, 0f, 1f),
                new Vec2(10, 1),
                new Vec2(10, 3)
            ),
            new GridColor(
                "White",
                new Color(1f, 1f, 1f),
                new Vec2(11, 1),
                new Vec2(11, 3)
            ),
            new GridColor(
                "Black",
                new Color(0f, 0f, 0f),
                new Vec2(12, 1),
                new Vec2(12, 3)
            ),
        };
    }

}