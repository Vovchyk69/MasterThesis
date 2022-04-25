/* eslint-disable no-undef */
import React from 'react'
import { Calendar, momentLocalizer } from 'react-big-calendar'
import moment from 'moment'
import 'react-big-calendar/lib/css/react-big-calendar.css'
import UserFront from '@userfront/react'
import { Navigate } from 'react-router-dom'
import axios from 'axios'
import withDragAndDrop from 'react-big-calendar/lib/addons/dragAndDrop'
import 'react-big-calendar/lib/addons/dragAndDrop/styles.css'
import { LoadingSpinner } from './LoadingSpinner'
import { CustomEvent } from './CustomEvent'
import '../App.css'
const localizer = momentLocalizer(moment)
const DnCalendar = withDragAndDrop(Calendar)

export class CustomCalendar extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      myEventList: [],
      isLoading: true,
      files: [],
    }

    this.moveEvent = this.moveEvent.bind(this)
    this.calculate = this.calculate.bind(this)
  }

  mapReservation(reservation) {
    const date = new Date(2022, 3, 25, 8, 0, 0)
    const startDate = new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDate() + parseInt(reservation.value.day),
      date.getHours() + parseInt(reservation.value.time)
    )

    const endDate = new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDate() + parseInt(reservation.value.day),
      date.getHours() +
        parseInt(reservation.value.time) +
        parseInt(reservation.key.duration)
    )

    const event = {
      startDate: startDate,
      endDate: endDate,
      title: `${reservation?.key?.studentCourse?.name}`,
      professor: `${reservation?.key?.studentProfessor?.name}`,
      groups: `${reservation?.key.studentGroups[0].name}`,
      room: `${reservation?.value?.studentRoom.name}`,
      color: '#b5c9c9',
    }

    return event
  }

  fetchFiles() {
    const url = process.env.REACT_APP_FILE_MANAGEMENT_URL
    axios
      .get(`${url}/documents`)
      .then((response) => {
        this.setState({ files: response.data })
      })
      .catch(() => {
        this.setState({ isLoading: false })
      })
  }

  sendRequest(fileName) {
    const url = process.env.REACT_APP_SCHEDULER_URL
    axios
      .post(`${url}/scheduler`, {
        fileName: fileName || 'data.json',
      })
      .then((response) => {
        this.setState({
          isLoading: false,
          myEventList: response.data.reservations.map((reservation) => {
            return this.mapReservation(reservation)
          }),
        })
      })
  }

  componentDidMount() {
    Promise.all([this.fetchFiles(), this.sendRequest()])
  }

  onEventResize(resizeType, { event, start, end }) {
    const { myEventList } = this.state

    const nextEvents = myEventList.map((existingEvent) => {
      return existingEvent.id == event.id
        ? { ...existingEvent, start, end }
        : existingEvent
    })

    this.setState({
      myEventList: nextEvents,
    })
  }

  moveEvent({ event, start, end }) {
    const { myEventList } = this.state

    const idx = myEventList.indexOf(event)
    const updatedEvent = { ...event, startDate: start, endDate: end }

    myEventList[idx] = updatedEvent

    this.setState({ myEventList: myEventList })
  }

  calculate(e) {
    const fileName = e.target.textContent
    this.setState({ myEventList: [], isLoading: true })
    this.sendRequest(fileName)
  }

  render() {
    if (!UserFront.accessToken()) {
      return (
        <Navigate
          to={{
            pathname: '/login',
          }}
        />
      )
    }

    const { myEventList, isLoading, files } = this.state
    if (isLoading) return <LoadingSpinner />

    return (
      <div style={{ minHeight: '600px' }}>
        <div
          style={{
            width: '100px',
            marginLeft: '110px',
            marginTop: '70px',
            verticalAlign: 'middle',
          }}
        >
          {files.map((file) => {
            return (
              <p
                key={file}
                style={{
                  maxLen: '10px',
                  textOverflow: 'ellipsis',
                  whiteSpace: 'nowrap',
                  overflow: 'hidden',
                }}
              >
                <a
                  href="#"
                  onClick={this.calculate}
                  style={{ textDecoration: 'none' }}
                >
                  {file}
                </a>
              </p>
            )
          })}
        </div>
        <DnCalendar
          style={{
            height: '100vh',
            marginLeft: '220px',
            top: 0,
            left: 0,
            padding: '20px',
            position: 'absolute',
            width: '80%',
          }}
          views={['week', 'day', 'work_week']}
          defaultView={'work_week'}
          step={30}
          localizer={localizer}
          events={myEventList}
          startAccessor="startDate"
          endAccessor="endDate"
          defaultDate={moment().toDate()}
          min={new Date(2022, 1, 0, 8, 0, 0)}
          max={new Date(2022, 1, 0, 19, 0, 0)}
          onEventResize={this.onEventResize}
          onEventDrop={this.moveEvent}
          components={{ work_week: { event: CustomEvent } }}
          eventPropGetter={(event) => {
            const eventData = myEventList.find((ot) => ot.id === event.id)
            const backgroundColor = eventData && eventData.color
            return {
              style: {
                backgroundColor,
                fontSize: '10px',
                justifyContent: 'center',
                alignItems: 'center',
                color: '#151717',
              },
            }
          }}
        />
      </div>
    )
  }
}
