import React, { Component } from 'react'
import { ThreeDots } from 'react-loader-spinner'
import '../spinner.css'

export class LoadingSpinner extends Component {
  constructor(props) {
    super(props)
    this.state = {}
  }

  render() {
    return (
      <div className="spinner">
        <ThreeDots color="grey" height="100" width="100" />
      </div>
    )
  }
}
