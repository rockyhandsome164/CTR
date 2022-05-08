const NoteList = () => {

    //CSS for NoteView
    const note = {
        backgroundColor: '#fef68a',
        borderRadius: '10px',
        padding: '1rem',
        minHeight: '170px',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
        whiteSpace: 'pre-wrap'
    }
    const noteFooter = {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
    }

    const text = 'Note';
    const date = new Date();

    return (
        <>
            <div style={note}>
                <span>{text}</span>
                <div style={noteFooter}>
                    <button
                        type="button"
                        className="input-group-addon"
                        onClick={() => handleDeleteNote(id)}
                    >
                        <i className="glyphicon glyphicon-trash"></i>
                    </button>
                </div>
            </div>



        </>)
}