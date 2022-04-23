import React, { Component } from 'react'
import UserFront from '@userfront/react'
import { Navigate } from 'react-router-dom'
import { UploadDoc } from './UploadFile'
import '../Upload.css'

export class Home extends Component {
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
      <div className="App">
        <div className="Card">
          <UploadDoc />
        </div>
      </div>
    )
  }
}
