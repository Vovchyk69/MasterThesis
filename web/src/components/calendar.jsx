import React from 'react'
import { Calendar, momentLocalizer } from 'react-big-calendar'
import moment from 'moment'
import 'react-big-calendar/lib/css/react-big-calendar.css'
import UserFront from '@userfront/react'
import { Navigate } from 'react-router-dom'
import axios from 'axios'
import withDragAndDrop from 'react-big-calendar/lib/addons/dragAndDrop'
import { LoadingSpinner } from './LoadingSpinner'

const localizer = momentLocalizer(moment)
const DnCalendar = withDragAndDrop(Calendar)

export class CustomCalendar extends React.Component {
  state = {
    myEventList: [],
    isLoading: true,
  }

  mapReservation(reservation) {
    const date = new Date(2022, 4, 25, 8, 0, 0)
    const startDate = date.setDate(
      date.getDay() + reservation.value.day,
      date.getHours() + reservation.value.time
    )

    const endDate = date.setDate(
      date.getDay() + reservation.value.day,
      date.getHours() + reservation.value.time + reservation.key.duration
    )

    const event = {
      start: startDate,
      end: endDate,
      title: reservation?.key?.course?.name || '',
      color: 'fff600',
    }

    return event
  }

  sendRequest() {
    const url = process.env.REACT_APP_SCHEDULER_URL
    axios
      .post(`${url}/scheduler`, {
        fileName: 'data.json',
      })
      .then((response) => {
        this.setState({
          isLoading: false,
          myEventList: response.data.reservations.map((reservation) =>
            this.mapReservation(reservation)
          ),
        })
      })
  }

  componentDidMount() {
    this.sendRequest()
  }

  onEventResize(data) {
    const { start, end } = data

    this.setState((state) => {
      state.myEventList[0].start = start
      state.myEventList[0].end = end
      return { myEventList: state.myEventList }
    })
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

    const { myEventList, isLoading } = this.state

    if (isLoading) return <LoadingSpinner />

    return (
      <div>
        <DnCalendar
          style={{
            height: '100vh',
            marginLeft: '120px',
            top: 0,
            left: 0,
            padding: '20px',
            position: 'absolute',
            width: '85%',
          }}
          views={['week', 'day', 'work_week']}
          defaultView={'work_week'}
          step={30}
          localizer={localizer}
          events={myEventList}
          startAccessor="start"
          endAccessor="end"
          defaultDate={moment().toDate()}
          min={new Date(2022, 1, 0, 8, 0, 0)}
          max={new Date(2022, 1, 0, 19, 0, 0)}
          onEventResize={this.onEventResize}
          eventPropGetter={(event) => {
            //const eventData = list.find((ot) => ot.id === event.id)
            const backgroundColor = event && event.color
            return { style: { backgroundColor } }
          }}
        />
      </div>
    )
  }
}
