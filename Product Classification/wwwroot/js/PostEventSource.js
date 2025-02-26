/**
 * PostEventSource - A browser-compatible EventSource alternative with POST support
 * Use this instead of SSE.js for better browser compatibility
 */
class PostEventSource {
    constructor(url, options = {}) {
        this.url = url;
        this.options = options;
        this.readyState = 0; // 0: CONNECTING, 1: OPEN, 2: CLOSED
        this.eventListeners = {};
        this.reconnectInterval = options.reconnectInterval || 3000;
        this.lastEventId = '';
        this.controller = null;

        // Start connection immediately if autoConnect is true (default)
        if (options.autoConnect !== false) {
            this.connect();
        }
    }

    connect() {
        // Don't reconnect if explicitly closed
        if (this.readyState === 2 && this.controller === null) {
            return;
        }

        this.readyState = 0;
        this.controller = new AbortController();

        const fetchOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'text/event-stream',
                'Cache-Control': 'no-cache'
            },
            body: JSON.stringify(this.options.payload || {}),
            signal: this.controller.signal,
            credentials: this.options.withCredentials ? 'include' : 'same-origin'
        };

        // Add last event ID if available
        if (this.lastEventId) {
            fetchOptions.headers['Last-Event-ID'] = this.lastEventId;
        }

        // Start the fetch
        fetch(this.url, fetchOptions)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error: ${response.status}`);
                }

                if (!response.body) {
                    throw new Error('ReadableStream not supported in this browser');
                }

                // Mark connection as open
                this.readyState = 1;
                this._dispatchEvent('open', { type: 'open' });

                // Set up the reader
                const reader = response.body.getReader();

                const decoder = new TextDecoder('utf-8');
                let buffer = '';

                // Process the stream
                const processStream = ({ done, value }) => {
                    if (done) {
                        // Connection closed normally
                        this._reconnect();
                        return;
                    }

                    // Decode the received chunk and add to buffer
                    buffer += decoder.decode(value, { stream: true });

                    // Process complete events (separated by double newlines)
                    const lines = buffer.split('\n\n');
                    buffer = lines.pop() || ''; // Keep the incomplete event in the buffer

                    // Process each complete event
                    for (const eventString of lines) {
                        if (!eventString.trim()) continue;
                        this._parseEvent(eventString);
                    }

                    // Continue reading
                    reader.read().then(processStream).catch(error => {
                        if (error.name !== 'AbortError') {
                            this._dispatchEvent('error', { type: 'error', data: error.message });
                            this._reconnect();
                        }
                    });
                };

                // Start reading the stream
                reader.read().then(processStream).catch(error => {
                    if (error.name !== 'AbortError') {
                        this._dispatchEvent('error', { type: 'error', data: error.message });
                        this._reconnect();
                    }
                });
            })
            .catch(error => {
                if (error.name !== 'AbortError') {
                    this._dispatchEvent('error', { type: 'error', data: error.message });
                    this._reconnect();
                }
            });
    }

    addEventListener(type, callback) {
        if (!this.eventListeners[type]) {
            this.eventListeners[type] = [];
        }
        this.eventListeners[type].push(callback);
    }

    removeEventListener(type, callback) {
        if (!this.eventListeners[type]) return;

        const index = this.eventListeners[type].indexOf(callback);
        if (index !== -1) {
            this.eventListeners[type].splice(index, 1);
        }
    }

    close() {
        if (this.controller) {
            this.controller.abort();
            this.controller = null;
        }
        this.readyState = 2;
        this._dispatchEvent('close', { type: 'close' });
    }

    // Private methods
    _dispatchEvent(type, event) {
        if (this.eventListeners[type]) {
            this.eventListeners[type].forEach(callback => {
                try {
                    callback(event);
                } catch (e) {
                    console.error('Error in event listener:', e);
                }
            });
        }

        // For the 'message' event, also call the onmessage handler if defined
        if (type === 'message' && typeof this.onmessage === 'function') {
            try {
                this.onmessage(event);
            } catch (e) {
                console.error('Error in onmessage handler:', e);
            }
        }

        // Special handling for other standard handlers
        const handlerName = 'on' + type;
        if (typeof this[handlerName] === 'function' && type !== 'message') {
            try {
                this[handlerName](event);
            } catch (e) {
                console.error(`Error in ${handlerName} handler:`, e);
            }
        }
    }

    _parseEvent(eventString) {
        const lines = eventString.split('\n');
        let eventType = 'message';
        let data = '';
        let eventId = '';
        let retry = null;

        // Parse the event data
        for (const line of lines) {
            if (!line.trim()) continue; // Skip empty lines

            // Parse field:value format
            const colonIndex = line.indexOf(':');
            if (colonIndex === -1) {
                // Field with no value
                continue;
            }

            const field = line.slice(0, colonIndex);
            // Skip the colon and the space after it (if present)
            const value = line.slice(colonIndex + 1).replace(/^ /, '');

            switch (field) {
                case 'event':
                    eventType = value;
                    break;
                case 'data':
                    data = data ? data + '\n' + value : value;
                    break;
                case 'id':
                    eventId = value;
                    break;
                case 'retry':
                    retry = parseInt(value, 10);
                    break;
            }
        }

        // Update the last event ID if present
        if (eventId) {
            this.lastEventId = eventId;
        }

        // Update reconnect interval if specified
        if (retry && !isNaN(retry)) {
            this.reconnectInterval = retry;
        }

        // Dispatch the event
        if (data) {
            this._dispatchEvent(eventType, {
                type: eventType,
                data: data,
                lastEventId: this.lastEventId
            });
        }
    }

    _reconnect() {
        // Only reconnect if:
        // 1. We're not already closed
        // 2. We haven't been explicitly closed
        if (this.readyState !== 2 || this.controller !== null) {
            this.readyState = 2;
            this._dispatchEvent('close', { type: 'close' });

            setTimeout(() => {
                this.connect();
            }, this.reconnectInterval);
        }
    }
}