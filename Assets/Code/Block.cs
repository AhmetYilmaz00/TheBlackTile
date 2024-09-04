using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    private const string _destroy_t = "Destroy";
    private const string _moving_b = "Moving";
    private const string _selected_b = "Selected";
    public int Multiplier { get; set; }

    public bool IsDefender;
    public bool IsMinusBlock;
    public bool IsMultiplier;

    [Header("Components")]
    public MeshRenderer Renderer;
    public TMP_Text Text;
    public Animator Animator;
    public GameObject HighlightSprite;
    public GameObject Shadow;
    public ParticleSystem SelectedParticle;
    public ParticleSystem IdleParticle;

    [Space(5)]
    public GameObject Face;
    public Animator FaceAnimator;

    public int Number;
    [HideInInspector]
    public Vector2Int Position;

    private FracturedBlock _fracture;

    private void Awake()
    {
        _fracture = GetComponent<FracturedBlock>();
    }

    public void Setup(int number)
    {
        Number = number;
        Text.text = number.ToString();
        if (!IsDefender && !IsMinusBlock && !IsMultiplier)
        {
            Renderer.sharedMaterial = AssetsConfiguration.instance.ColorTheme.BlockMaterials[
                (number - 1) % AssetsConfiguration.instance.ColorTheme.BlockMaterials.Length
                ];
        }
        else if (IsDefender && number >= 0)
        {
            int materialIndex;
            for (materialIndex = 0; materialIndex < AssetsConfiguration.instance.ColorTheme.DefenderBlockMaterials.Length; materialIndex++)
            {
                if (number < AssetsConfiguration.instance.DefenderBlockNumbersToChangeMaterial[materialIndex])
                {
                    materialIndex--;
                    break;
                }
            }
            if (materialIndex >= AssetsConfiguration.instance.DefenderBlockNumbersToChangeMaterial.Length)
                materialIndex--;

            Renderer.sharedMaterial = AssetsConfiguration.instance.ColorTheme.DefenderBlockMaterials[materialIndex];
            IdleParticle.GetComponent<ParticleSystemRenderer>().sharedMaterial = Renderer.sharedMaterial;
        }
        else if(IsMinusBlock)
        {
            Renderer.sharedMaterial = AssetsConfiguration.instance.ColorTheme.MinusBlockMaterial;
        }

        if (IsMinusBlock)
        {
            Text.color = AssetsConfiguration.instance.ColorTheme.MinusBlocksFontColor;
        }
        else if (IsDefender)
        {
            Text.color = AssetsConfiguration.instance.ColorTheme.DefenderBlockFontColor;
        }
        else
        {
            Text.color = AssetsConfiguration.instance.ColorTheme.BlocksFontColor;
        }

        if(FaceAnimator != null && IsMinusBlock)
        {
            FaceAnimator.Play("Angry");
        }
    }

    public void SetupNumberPreviewOnly(List<Block> selectedBlocks)
    {
        int number = 0;
        for (int i = 0; i < selectedBlocks.Count; i++)
        {
            if(selectedBlocks[i].IsMultiplier)
            {
                number *= selectedBlocks[i].Multiplier;
            }
            else
            {
                number += selectedBlocks[i].Number;
            }
        }

        Text.text = number.ToString();
        if(number >= 0)
        {
            Text.color = AssetsConfiguration.instance.ColorTheme.DefenderBlockFontColor;
        }
        else
        {
            Text.color = Color.red;
        }
    }

    public void ClearNumberPreview()
    {
        Text.text = Number.ToString();
        Text.color = AssetsConfiguration.instance.ColorTheme.DefenderBlockFontColor;
    }

    public void SetupMultiplier(int multiplier)
    {
        Multiplier = multiplier;
        Text.text = $"X{Multiplier}";
    }

    public void SetSelected(bool selected)
    {
        HighlightSprite.SetActive(selected);
        Animator.SetBool(_selected_b, selected);
        if (selected)
        {
            SelectedParticle.GetComponent<ParticleSystemRenderer>().sharedMaterial = Renderer.sharedMaterial;
            SelectedParticle.Play();
        }
    }

    public void SetMoving(bool moving)
    {
        Animator.SetBool(_moving_b, moving);
    }

    public void DestroyOnLose()
    {
        Text.color = Color.red;

        if (FaceAnimator != null)
        {
            FaceAnimator.Play("Angry");
        }

        Animator.SetTrigger(_destroy_t);
        Invoke(nameof(FractureOnDestroy), 1f);
    }

    public void FractureOnDestroy()
    {
        Renderer.gameObject.SetActive(false);
        Text.gameObject.SetActive(false);
        Shadow.SetActive(false);
        HighlightSprite.SetActive(false);
        _fracture.SetMaterial(Renderer.sharedMaterial);
        _fracture.DestroyBlock();
        if (Face != null)
        {
            Destroy(Face);
        }
        Destroy(this);
        Destroy(GetComponent<Collider>());
        Destroy(gameObject, 3f);
    }
}