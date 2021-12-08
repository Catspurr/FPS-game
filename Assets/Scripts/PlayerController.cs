using System;
using System.Runtime.Remoting.Messaging;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private GameObject cameraRig, UICanvas, scoreboard, escapeMenu;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private float mouseSensitivity, walkSpeed, sprintSpeed, jumpForce, smoothTime;

    [SerializeField] private Item[] items;

    private int _itemIndex, _previousItemIndex = -1;

    private float _verticalLookRotation;
    private bool _grounded;
    private Vector3 _smoothMoveVelocity, _moveAmount;
    
    private Rigidbody _rigidbody;
    private PhotonView _photonView;

    private const float _maxHealth = 100f;
    private float _currentHealth = _maxHealth;

    private PlayerManager _playerManager;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();

        _playerManager = PhotonView.Find((int)_photonView.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            EquipItem(0);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(_rigidbody);
            Destroy(UICanvas);
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;
        if (!escapeMenu.activeSelf)
        {
            Look();
            Move();
            Items();
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }
        
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapeMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine) return;
        _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);

        cameraRig.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
    }

    private void Move()
    {
        var moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        _moveAmount = Vector3.SmoothDamp(
            _moveAmount,
            moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed),
            ref _smoothMoveVelocity,
            smoothTime);

        if (Input.GetKeyDown(KeyCode.Space) && _grounded)
        {
            _rigidbody.AddForce(transform.up * jumpForce);
        }
    }

    private void Items()
    {
        for (var i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (_itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(_itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (_itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(_itemIndex - 1);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[_itemIndex].Use();
        }
    }
    public void SetGroundedState(bool grounded)
    {
        _grounded = grounded;
    }

    private void EquipItem(int index)
    {
        if (index == _previousItemIndex) return;
        
        _itemIndex = index;
        items[_itemIndex].itemGameObject.SetActive(true);

        if (_previousItemIndex != -1)
        {
            items[_previousItemIndex].itemGameObject.SetActive(false);
        }

        _previousItemIndex = _itemIndex;

        if (_photonView.IsMine)
        {
            var hash = new Hashtable();
            hash.Add("itemIndex", _itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!_photonView.IsMine && targetPlayer == _photonView.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void TakeDamage(float damage)
    {
        _photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage)
    {
        if (!_photonView.IsMine) return;

        _currentHealth -= damage;

        healthBarImage.fillAmount = _currentHealth / _maxHealth;
        
        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        _playerManager.Die();
    }
}