let popup;

function buttonClicked(conversationId, lastName) {
    var me = new Talk.User({
        id: 'Proctor@gmail.com',
        name: 'Proctor@gmail.com',
        welcomeMessage: 'Hey, how can I help?',
    });

    var student = new Talk.User({
        id: lastName,
        name: lastName,
    });

    window.talkSession = new Talk.Session({
        appId: 't0izOhm6',
        me: me,
    });
    // assume `session` has been set elsewhere
    const conversation = window.talkSession.getOrCreateConversation(conversationId);
    conversation.setParticipant(me);
    conversation.setParticipant(student);

    if (popup) {
        popup.show(); //in case popup is hidden
        popup.select(conversation); //select the conversation clicked on
    } else {
        //if there is no existing popup, create one
        popup = window.talkSession.createPopup(conversation);
        popup.mount({ show: true });
    }
}