import React, { Component } from 'react'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import { Home } from './Home'
import { CustomCalendar } from './calendar'
import { Sidebar } from './sidebar'
import { Login } from './Login'
import '../App.css'

export class App extends Component {
  render() {
    return (
      <Router>
        <Sidebar></Sidebar>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/login" element={<Login />} exact />
          <Route path="/home" element={<Home />} />
          <Route path="/calendar" element={<CustomCalendar />} />
          <Route path="/logout" element={<Login />} />
        </Routes>
      </Router>
    )
  }
}
