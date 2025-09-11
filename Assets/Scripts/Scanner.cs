using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class Scanner : MonoBehaviour
{
    PlayerInputHandler playerInputHandler;
    bool isScanning;
    float scanTimeElapsed;
    [SerializeField] float timeBetweenScans = .1f;
    [SerializeField] float angleSpread;

    [SerializeField] LayerMask scanMask;

    [SerializeField] float spreadChangeSpeed;
    [SerializeField] float minSpread = 5f;
    [SerializeField] float maxSpread = 30f;

    [SerializeField] AudioSource audioSource;

    ScanLinesPool scanLinesPool;

    [SerializeField] TemporaryScanDot temporaryScanDotPrefab;

    SpawnablesManager spawnablesManager;
    private void Awake()
    {
        scanLinesPool = GetComponentInChildren<ScanLinesPool>();
        playerInputHandler = GetComponentInParent<PlayerInputHandler>();
    }
    private void Start()
    {
        spawnablesManager = GameManager.instance.spawnablesManager;
    }
    public void SetIsScanning(bool isScanning)
    {
        this.isScanning = isScanning;
    }
    public void ChangeSpread(float spreadDelta)
    {
        angleSpread = Mathf.Clamp(angleSpread+spreadChangeSpeed * spreadDelta, minSpread, maxSpread);
        
    }

    private void Update()
    {
        isScanning = playerInputHandler.scan;
        if (isScanning) 
        {
            ScanTerrain();
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
    public void ScanTerrain()
    {
        scanTimeElapsed += Time.deltaTime;
        Vector3 scanPosition = Camera.main.transform.position;
        Vector3 forwardDirection = Camera.main.transform.forward;
        while (scanTimeElapsed >= timeBetweenScans)
        {
            float randomSeed = Random.Range(-angleSpread, angleSpread);
            float randomAngle = Random.Range(0, 2 * Mathf.PI);

           

            // Generate a random direction within angleSpread of transform.forward
            Vector3 randomDirection = Quaternion.AngleAxis(randomSeed*Mathf.Cos(randomAngle), transform.right)*Quaternion.AngleAxis(randomSeed*Mathf.Sin(randomAngle), transform.up) * forwardDirection;
            
            // Perform the raycast  
            RaycastHit hit;
            if (Physics.Raycast(scanPosition, randomDirection, out hit, scanMask))
            {
                if(hit.collider.gameObject.layer == GameManager.creatureScansLayer) //collider.TryGetComponent(out LightSensitiveComponent lsc))
                {
                    hit.collider.GetComponentInParent<LightSensitiveComponent>().ReceiveScan(randomDirection, scanPosition);
                    spawnablesManager.SpawnEntity(temporaryScanDotPrefab, hit.point, Quaternion.identity);
                    //Instantiate(temporaryScanDotPrefab, hit.point, Quaternion.identity);
                }
                else
                {
                    DotManager.instance.SpawnDot(hit.point);
                    ScanLine scanLine = scanLinesPool.Get();
                    scanLine.SetLine(hit.point);
                }
                
            }
            else
            {
                ScanLine scanLine = scanLinesPool.Get();
                scanLine.SetLine(transform.position + randomDirection * 100f);
            }
            scanTimeElapsed -= timeBetweenScans;
        }
    }
}
