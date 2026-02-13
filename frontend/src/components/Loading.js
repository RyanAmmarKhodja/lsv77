import React from 'react'

const Loading = ({ loading }) => {
  return (
    loading && (
    <div className="spinner-border mt-5" role="status">
        <span className="sr-only"></span>
      </div>
    )
  )
}

export default Loading