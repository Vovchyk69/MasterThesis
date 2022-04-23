import React from 'react'
import { Calendar, momentLocalizer } from 'react-big-calendar'
import moment from 'moment'
import 'react-big-calendar/lib/css/react-big-calendar.css'
import UserFront from '@userfront/react'
import { Navigate } from 'react-router-dom'

const localizer = momentLocalizer(moment)

export class CustomCalendar extends React.Component {
  render() {
    if (!UserFront.accessToken()) {
      return (
        <Navigate
          to={{
            pathname: '/login',
            state: { from: location },
          }}
        />
      )
    }

    return (
      <div>
        <Calendar
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
          events={undefined}
          startAccessor="start"
          endAccessor="end"
          defaultDate={moment().toDate()}
          min={new Date(2022, 1, 0, 8, 0, 0)}
          max={new Date(2022, 1, 0, 19, 0, 0)}
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
