import React, { Component } from 'react'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import { Home } from './Home'
import { CustomCalendar } from './calendar'
import { Sidebar } from './sidebar'
import { Login } from './Login'
import '../App.css'

export class App extends Component {
  constructor(props) {
    super(props)
    this.state = {
      data: [],
    }
  }

  fetchWeather = () => {
    // fetch('https://localhost:7212/weatherforecast')
    //   .then((response) => response.json())
    //   .then((booksList) => {
    //     this.setState({ data: booksList });
    //   })
    //   .catch((e) => {
    //     console.log(e);
    //   });
  }

  componentDidMount() {
    //this.fetchWeather();
  }

  render() {
    // const { data } = this.state;
    // const url = process.env.REACT_APP_BACKEND_URL;
    // console.log(url);

    // return (
    //   <div className="App">
    //     <Sidebar />
    //   </div>
    // );
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
