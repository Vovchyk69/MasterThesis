import React from 'react'
import '../App.css'
export class CustomEvent extends React.Component {
  render() {
    const { event } = this.props
    return (
      <div
        className="element"
        style={{
          justifyContent: 'center',
          color: '#151717',
          overflowY: 'auto',
          height: 'inherit',
          scrollbarWidth: '2px',
        }}
      >
        <p>{event.title}</p>
        <p>Professor {event.professor}</p>
        <p>Groups {event.groups}</p>
        <p>Room {event.room}</p>
      </div>
    )
  }
}
