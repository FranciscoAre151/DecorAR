using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CategoryDropdownManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown categoryDropdown; // Dropdown asignado desde el Inspector
    [SerializeField] private List<string> categories; // Lista de categorías configurada desde el Inspector

    // Evento o acción que se ejecutará al seleccionar una categoría
    public delegate void OnCategorySelected(string category);
    public static event OnCategorySelected CategorySelected;

    void Start()
    {
        InitializeDropdown();
        categoryDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    /// <summary>
    /// Inicializa el dropdown con las categorías proporcionadas.
    /// </summary>
    private void InitializeDropdown()
    {
        categoryDropdown.ClearOptions(); // Limpiar opciones previas
        categoryDropdown.AddOptions(categories); // Agregar las categorías al dropdown
    }

    /// <summary>
    /// Acción a ejecutar cuando se selecciona un valor del dropdown.
    /// </summary>
    /// <param name="index">Índice seleccionado.</param>
    private void OnDropdownValueChanged(int index)
    {
        string selectedCategory = categories[index];
        Debug.Log($"Categoría seleccionada: {selectedCategory}");

        // Disparar el evento o acción
        CategorySelected?.Invoke(selectedCategory);
    }

    /// <summary>
    /// Permite actualizar dinámicamente las categorías en el dropdown.
    /// </summary>
    /// <param name="newCategories">Lista de nuevas categorías.</param>
    public void UpdateCategories(List<string> newCategories)
    {
        categories = newCategories;
        InitializeDropdown();
    }

    /// <summary>
    /// Obtiene la categoría actualmente seleccionada.
    /// </summary>
    /// <returns>Nombre de la categoría seleccionada.</returns>
    public string GetSelectedCategory()
    {
        return categories[categoryDropdown.value];
    }
}