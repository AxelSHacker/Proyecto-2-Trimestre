using UnityEngine;

[CreateAssetMenu(fileName = "Armas_SO", menuName = "Scriptable Objects/Armas_SO")]
public class Armas_SO : ScriptableObject
{
    [Header("Datos del Arma")]
    public string nombre;
    public int animatorID;
    public string bulletPoolID;

    [Header("Configuracion Visual")]
    public int moidelIndex;

    [Header("IK Rigging(Manos)")]
    public int posiciodelasManos;

}
