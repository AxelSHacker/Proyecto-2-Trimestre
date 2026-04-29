using UnityEngine;

[CreateAssetMenu(fileName = "Armas_SO", menuName = "Scriptable Objects/Armas_SO")]
public class Armas_SO : ScriptableObject
{
    [Header("Datos del Arma")]
    public string nombre;
    public string shootTrigger;
    public string recargarTrigger;
    public float fireRate;
    public string bulletPoolID;
    public bool esMisil;
    public bool usarEnfriamiento;
    public AnimatorOverrideController overrideController;

    [Header("Configuracion Visual")]
    public int modelIndex;
    public float multiplicadorAnimacion = 1f;

    [Header("Punto de disparo")]
    public string nombreShootPoint;

    [Header("Municion")]
    public int capacidadCargador;
    public int municionTotal;

}
