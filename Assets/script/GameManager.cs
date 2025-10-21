using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject Bienvenida;   // Asigna tu Canvas del cartel en el inspector
    public Button buttonNext;      // Botón "Siguiente"
    public Button buttonClose;     // Botón "X"
    private int infoPantalla = 0;
    private bool isPaused = false;
    public UnityEvent infoMov;
    public UnityEvent infoSprin;
    public UnityEvent infoAtack;
    public UnityEvent infoDef;
    private Animator animator;

    void Start()
    {
        // Asignar los eventos a los botones
        buttonNext.onClick.AddListener(OnNextButtonClick);
        buttonClose.onClick.AddListener(CloseInfoPanel);
        animator = GetComponent<Animator>();
        // Si el cartel debe aparecer al inicio

    }

    public void OnNextButtonClick()
    {

        switch (infoPantalla)
        {
            case 0:
                // Mostrar panel principal y primer paso (movimiento)
                ShowInfoPanel();
                infoMov?.Invoke();
                infoPantalla += 1;
                break;

            case 1:
                // Segundo paso: sprint
                infoSprin?.Invoke();
                infoPantalla += 1;
                break;

            case 2:
                // Tercer paso: ataque
                infoAtack?.Invoke();
                infoPantalla += 1;
                break;

            case 3:
                // Cuarto paso: defensa
                infoDef?.Invoke();
                infoPantalla += 1;
                break;

            default:
                // Fin del tutorial: cerrar panel y reanudar juego
                CloseInfoPanel();
                break;
        }

    }

    public void ShowInfoPanel()
    {
        animator.Play("panel_Tutorial");
        Bienvenida.SetActive(true);
        PauseGame();
    }

    public void CloseInfoPanel()
    {
        animator.SetTrigger("Hide");
        gameObject.SetActive(false);
        ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;   // Pausa todo lo que dependa de Time.deltaTime
        isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
