import React from 'react'

function PopupMenu({ locker, onClose }) {
  console.log('Locker:', locker);
  const buttonStyle = {
    display: 'block',
    width: '100%',
    marginBottom: '4px',
    textAlign: 'left',
    backgroundColor: '#ddd',
    border: 'none',
    cursor: 'pointer',
    padding: '4px 6px',
  };

  return (
    <div onClick={(e) => e.stopPropagation()}>
      <h4>Locker #{locker.id} (type: {locker.lockerType})</h4>

      <button style={buttonStyle} onClick={() => { console.log('Edit'); onClose(); }}> Edit</button>
      <button style={buttonStyle} onClick={() => { console.log('Open'); onClose(); }}>Open Locker</button>
      <button style={buttonStyle} onClick={() => { console.log('Suspend'); onClose(); }}>Suspend Locker</button>

      {locker.lockerType === 'Personal' && (
        <button style={buttonStyle} onClick={() => { console.log('Set'); onClose(); }}>Set User</button>
      )}

      {locker.lockerType === 'hand' && (
        <button onClick={() => { console.log('Hand action'); onClose(); }}>
          Hand action
        </button>
      )}

      {locker.lockerType === 'Parcel' && (
        <button style={buttonStyle} onClick={() => { console.log('Set'); onClose(); }}>Set User</button>
      )}
    </div>
  )
}

export default PopupMenu