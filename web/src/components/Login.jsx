import React, { Component } from 'react'
import UserFront from '@userfront/react'
import '../App.css'

UserFront.init('pn4xdpyn')
const SignupForm = UserFront.build({ toolId: 'doadno' })

export class Login extends Component {
  render() {
    return (
      <div className="App">
        <SignupForm />
      </div>
    )
  }
}
