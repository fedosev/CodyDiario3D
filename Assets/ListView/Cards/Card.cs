using UnityEngine;

namespace ListView {

    public class Card : ListViewItem<CardData> {

        public TextMesh topNum, botNum;
        public float centerScale = 3f;

        public GameObject face;

        public Material leftMaterial;
        public Material forwardMaterial;
        public Material rightMaterial;

        Vector3 m_Size;

        public override void Setup(CardData data) {

            base.Setup(data);

            m_Size = GetComponent<BoxCollider>().size;

            SetupCard();
        }

        void SetupCard() {

            var rend = face.GetComponent<Renderer>();
            switch (data.cardType) {
                case CardTypes.LEFT: rend.material = leftMaterial; break;
                case CardTypes.FORWARD: rend.material = forwardMaterial; break;
                case CardTypes.RIGHT: rend.material = rightMaterial; break;
            }
        }

    }

    [System.Serializable]     //Will cause warnings, but helpful for debugging
    public class CardData : ListViewItemData {
        public CardTypes cardType;
    }
}