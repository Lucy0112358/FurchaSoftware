import React from 'react'

function NoData({ text }) {
  const dataStyle = {
    display: 'flex',
    justifyContent: 'center',
    fontSize: '30px',
    color: 'rgb(170, 170, 170)',
  }
  return (
    <h1  style={dataStyle}>{text}</h1>
  )
}

export default NoData